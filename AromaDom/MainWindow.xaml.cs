using AromaDom.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AromaDom.Data;
using AromaDom.Models;

namespace AromaDom
{
    public partial class MainWindow : Window
    {
        private readonly MaterialRepository materialRepository = new MaterialRepository();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMaterials();
        }

        private void LoadMaterials()
        {
            try
            {
                MaterialsListBox.ItemsSource = materialRepository.GetMaterials();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "Не удалось загрузить список материалов из базы данных.\n" +
                    "Проверьте, что SQL Server запущен, а база AromaDom доступна.\n\n" +
                    exception.Message,
                    "Ошибка загрузки данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenEditWindow(null);
        }

        private void MaterialsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedMaterial = MaterialsListBox.SelectedItem as Material;

            if (selectedMaterial == null)
            {
                return;
            }

            OpenEditWindow(selectedMaterial.MaterialId);

            // снимаем выбор чтобы повторный клик по той же карточке снова открывал форму
            MaterialsListBox.SelectedItem = null;
        }

        private void OpenEditWindow(int? materialId)
        {
            var editWindow = new MaterialEditWindow(materialId);
            editWindow.Owner = this;
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                LoadMaterials();
            }
        }
    }
}