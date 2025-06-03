using Microsoft.Win32;
using OfficeOpenXml;
using PetShop.AppData;
using PetShop.Pages; // Добавьте эту директиву using
using System;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            // Преобразование коллекции в правильный тип и создание безопасного списка
            List<Products> allProducts = ((IEnumerable<object>)dgProducts.ItemsSource)?.Cast<Products>()?.ToList() ?? new List<Products>();

            // Проверяем наличие элементов
            if (!allProducts.Any())
            {
                MessageBox.Show("Нет товаров для экспорта.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ExportToCsv(allProducts);
        }

        private void ExportToCsv(List<Products> products)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV файлы (*.csv)|*.csv|Excel файлы (*.xlsx)|*.xlsx",
                    FileName = "Товары.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var sb = new StringBuilder();

                    // Добавляем заголовки столбцов
                    sb.AppendLine("Номер;Название;Цена;Количество;Категория;Производитель");

                    // Заполняем строки данными
                    foreach (var product in products)
                    {
                        // Обрабатываем случай, когда категории или фирмы отсутствуют
                        string categoryName = product.Categories != null ? product.Categories.Name : "";
                        string firmName = product.Firms != null ? product.Firms.Name : "";

                        sb.AppendLine(
                            $"{product.ProductId};{product.Name};{product.Price};{product.Quantity};{categoryName};{firmName}"
                        );
                    }

                    // Сохраняем файл
                    System.IO.File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Файл успешно сохранён!", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}