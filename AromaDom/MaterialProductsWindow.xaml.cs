using System;
using System.Collections.Generic;
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

namespace AromaDom
{
    /// <summary>
    /// Логика взаимодействия для MaterialProductsWindow.xaml
    /// </summary>
    public partial class MaterialProductsWindow : Window
    {
        private readonly MaterialRepository materialRepository = new MaterialRepository();
        private readonly int materialId;

        public MaterialProductsWindow(int materialId, string materialName)
        {
            InitializeComponent();
            this.materialId = materialId;
            MaterialNameTextBlock.Text = materialName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var products = materialRepository.GetProductsByMaterial(materialId);
                ProductsListBox.ItemsSource = products;

                if (products.Count == 0)
                {
                    ProductsListBox.Visibility = Visibility.Collapsed;
                    EmptyTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "Не удалось загрузить список продукции из базы данных.\n" +
                    "Проверьте подключение к SQL Server и повторите попытку.\n\n" +
                    exception.Message,
                    "Ошибка загрузки данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
