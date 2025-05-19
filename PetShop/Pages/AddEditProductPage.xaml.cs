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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PetShop.AppData;

namespace PetShop.Pages
{
    public partial class AddEditProductPage : Page
    {
        private PRODUCTS _currentProduct;

        public AddEditProductPage()
        {
            InitializeComponent();
            _currentProduct = new PRODUCTS();
            DataContext = _currentProduct;
            LoadComboBoxData();
        }

        public AddEditProductPage(PRODUCTS product)
        {
            InitializeComponent();
            _currentProduct = product;
            DataContext = _currentProduct;
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка категорий
                cmbCategories.ItemsSource = AppConnect.model0db.CATEGORIES.ToList();

                // Загрузка производителей
                cmbFirms.ItemsSource = AppConnect.model0db.FIRM.ToList();

                // Загрузка типов
                cmbTypes.ItemsSource = AppConnect.model0db.TYPE.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(_currentProduct.name) ||
                _currentProduct.price <= 0 ||
                _currentProduct.quantity < 0 ||
                _currentProduct.category_id == 0 ||
                _currentProduct.firm_id == 0 ||
                _currentProduct.type_id == 0)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentProduct.product_id == 0)
                AppConnect.model0db.PRODUCTS.Add(_currentProduct);

            try
            {
                AppConnect.model0db.SaveChanges();
                MessageBox.Show("Данные сохранены успешно!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.png)|*.jpg;*.png|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                _currentProduct.image = dialog.FileName;
                imgPreview.Source = new BitmapImage(new Uri(_currentProduct.image));
            }
        }
    }
}
