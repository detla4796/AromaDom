using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AromaDom.Data;
using AromaDom.Models;

namespace AromaDom
{
    public partial class MaterialEditWindow : Window
    {
        private const decimal MaxValue = 999999999m;
        private static readonly CultureInfo RuCulture = new CultureInfo("ru-RU");

        private readonly MaterialRepository materialRepository = new MaterialRepository();
        private readonly int? materialId;
        private bool isDirty;
        private bool isLoading = true;

        public MaterialEditWindow(int? materialId)
        {
            InitializeComponent();
            this.materialId = materialId;

            if (materialId.HasValue)
            {
                Title = "Редактирование материала — Арома Дом";
                HeaderTextBlock.Text = "Редактирование материала";
            }
            else
            {
                Title = "Добавление материала — Арома Дом";
                HeaderTextBlock.Text = "Новый материал";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TypeComboBox.ItemsSource = materialRepository.GetMaterialTypes();
                UnitComboBox.ItemsSource = materialRepository.GetUnits();

                if (materialId.HasValue)
                {
                    FillForm(materialRepository.GetMaterialById(materialId.Value));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "Не удалось загрузить данные материала из базы данных.\n" +
                    "Проверьте подключение к SQL Server и повторите попытку.\n\n" +
                    exception.Message,
                    "Ошибка загрузки данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
            finally
            {
                isLoading = false;
            }
        }

        private void FillForm(Material material)
        {
            if (material == null)
            {
                MessageBox.Show(
                    "Выбранный материал не найден в базе данных. Возможно, он был удалён.\n" +
                    "Вернитесь к списку и обновите его.",
                    "Материал не найден",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                Close();
                return;
            }

            TypeComboBox.SelectedValue = material.MaterialTypeId;
            NameTextBox.Text = material.MaterialName;
            PriceTextBox.Text = material.UnitPrice.ToString("0.00", RuCulture);
            UnitComboBox.SelectedValue = material.UnitId;
            PackageTextBox.Text = material.PackageQuantity.ToString("0.###", RuCulture);
            StockTextBox.Text = material.StockQuantity.ToString("0.###", RuCulture);
            MinimumTextBox.Text = material.MinimumQuantity.ToString("0.###", RuCulture);
        }

        private void Field_Changed(object sender, RoutedEventArgs e)
        {
            if (isLoading)
            {
                return;
            }

            isDirty = true;
            var field = sender as Control;

            if (field != null)
            {
                // при правке поля снимаем подсветку ошибки
                field.ClearValue(Control.BorderBrushProperty);
                field.ClearValue(Control.BorderThicknessProperty);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var errors = new List<string>();
            ResetHighlights();

            if (TypeComboBox.SelectedValue == null)
            {
                errors.Add("Тип материала: выберите тип из выпадающего списка.");
                Mark(TypeComboBox);
            }

            string name = NameTextBox.Text.Trim();

            if (name.Length == 0)
            {
                errors.Add("Наименование: поле не заполнено. Введите название материала.");
                Mark(NameTextBox);
            }

            decimal? price = ReadNumber(PriceTextBox, "Цена единицы", 2, true, errors);

            if (UnitComboBox.SelectedValue == null)
            {
                errors.Add("Единица измерения: выберите единицу из выпадающего списка.");
                Mark(UnitComboBox);
            }

            decimal? package = ReadNumber(PackageTextBox, "Количество в упаковке", 3, false, errors);
            decimal? stock = ReadNumber(StockTextBox, "Количество на складе", 3, true, errors);
            decimal? minimum = ReadNumber(MinimumTextBox, "Минимальное количество", 3, true, errors);

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    "Данные не сохранены. Исправьте следующие ошибки:\n\n• " +
                    string.Join("\n• ", errors),
                    "Проверьте введённые данные",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                int excludeId = materialId.HasValue ? materialId.Value : 0;

                if (materialRepository.IsNameTaken(name, excludeId))
                {
                    Mark(NameTextBox);
                    MessageBox.Show(
                        "Материал с названием «" + name + "» уже существует.\n" +
                        "Введите другое наименование.",
                        "Такой материал уже есть",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (materialId.HasValue)
                {
                    MessageBoxResult confirm = MessageBox.Show(
                        "Сохранить изменения в материале «" + name + "»?\n" +
                        "Прежние значения будут заменены.",
                        "Подтверждение изменения",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirm == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                var material = new Material
                {
                    MaterialName = name,
                    MaterialTypeId = (int)TypeComboBox.SelectedValue,
                    UnitId = (int)UnitComboBox.SelectedValue,
                    UnitPrice = price.Value,
                    PackageQuantity = package.Value,
                    StockQuantity = stock.Value,
                    MinimumQuantity = minimum.Value
                };

                if (materialId.HasValue)
                {
                    material.MaterialId = materialId.Value;
                    materialRepository.UpdateMaterial(material);
                }
                else
                {
                    materialRepository.AddMaterial(material);
                }

                isDirty = false;
                MessageBox.Show(
                    "Материал «" + name + "» сохранён.",
                    "Сохранение выполнено",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "Не удалось сохранить материал в базе данных.\n" +
                    "Проверьте подключение к SQL Server и повторите попытку.\n\n" +
                    exception.Message,
                    "Ошибка сохранения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!isDirty)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Введённые данные не сохранены и будут потеряны.\n" +
                "Закрыть форму без сохранения?",
                "Несохранённые изменения",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private decimal? ReadNumber(TextBox box, string fieldName, int decimals, bool zeroAllowed, List<string> errors)
        {
            // точку приравниваем к запятой, чтобы можно было вводить 12.50
            string text = box.Text.Trim().Replace('.', ',');
            decimal value = 0;

            if (text.Length == 0)
            {
                errors.Add(fieldName + ": поле не заполнено. Введите число, например 12,50.");
            }
            else if (!decimal.TryParse(text, NumberStyles.Number, RuCulture, out value))
            {
                errors.Add(fieldName + ": введено не число. Используйте только цифры и запятую, например 12,50.");
            }
            else if (value < 0)
            {
                errors.Add(fieldName + ": значение не может быть отрицательным. Введите 0 или больше.");
            }
            else if (value == 0 && !zeroAllowed)
            {
                errors.Add(fieldName + ": значение должно быть больше нуля.");
            }
            else if (decimal.Round(value, decimals) != value)
            {
                errors.Add(fieldName + ": допускается не более " + decimals + " знаков после запятой.");
            }
            else if (value > MaxValue)
            {
                errors.Add(fieldName + ": значение слишком большое. Максимум — 999 999 999.");
            }
            else
            {
                return value;
            }

            Mark(box);
            return null;
        }

        private void Mark(Control field)
        {
            field.BorderBrush = Brushes.Firebrick;
            field.BorderThickness = new Thickness(2);
        }

        private void ResetHighlights()
        {
            var fields = new Control[]
            {
                TypeComboBox, NameTextBox, PriceTextBox,
                UnitComboBox, PackageTextBox, StockTextBox, MinimumTextBox
            };

            foreach (Control field in fields)
            {
                field.ClearValue(Control.BorderBrushProperty);
                field.ClearValue(Control.BorderThicknessProperty);
            }
        }
    }
}
