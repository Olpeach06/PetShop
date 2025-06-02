using PetShop.AppData;
using PetShop.Pages; // Добавьте эту директиву using
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class AdminProductsPage : Page
    {
        private Frame _adminFrame; // Добавляем поле для хранения ссылки на фрейм

        public AdminProductsPage(Frame adminFrame) // Добавляем параметр в конструктор
        {
            InitializeComponent();
            _adminFrame = adminFrame; // Сохраняем ссылку на фрейм
            LoadProducts();
        }

        private void LoadProducts()
        {
            AppConnect.model0db.Products
                .Include(p => p.Categories)
                .Include(p => p.Firms)
                .Include(p => p.TypeOfPr)
                .Load();

            dgProducts.ItemsSource = AppConnect.model0db.Products.Local;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _adminFrame.Navigate(new AddEditProductPage()); // Используем сохранённый фрейм
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = dgProducts.SelectedItem as Products;
            if (selectedProduct != null)
            {
                _adminFrame.Navigate(new AddEditProductPage(selectedProduct));
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования!");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = dgProducts.SelectedItem as Products;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для удаления.");
                return;
            }

            // Подтверждение удаления
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить товар \"{selectedProduct.Name}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    AppConnect.model0db.Products.Remove(selectedProduct); // Удаляем продукт из базы
                    AppConnect.model0db.SaveChanges(); // Сохраняем изменения
                    LoadProducts(); // Обновляем список товаров
                    MessageBox.Show("Товар успешно удалён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}