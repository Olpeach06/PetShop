using PetShop.AppData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class CatalogPage : Page
    {
        private Users _currentUser;
        private List<Products> _allProducts;

        public CatalogPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _allProducts = AppConnect.model0db.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Firms)
                    .Include(p => p.TypeOfPr)
                    .ToList();

                // Загрузка категорий с пунктом "По умолчанию"
                var categories = AppConnect.model0db.Categories.ToList();
                cmbCategories.Items.Clear();
                cmbCategories.Items.Add(new Categories { Name = "по умолчанию", CategoryId = -1 }); // Первый элемент
                foreach (var cat in categories)
                    cmbCategories.Items.Add(cat);
                cmbCategories.SelectedIndex = 0;

                // Загрузка фирм с пунктом "По умолчанию"
                var firms = AppConnect.model0db.Firms.ToList();
                cmbFirms.Items.Clear();
                cmbFirms.Items.Add(new Firms { Name = "по умолчанию", FirmId = -1 }); // Первый элемент
                foreach (var firm in firms)
                    cmbFirms.Items.Add(firm);
                cmbFirms.SelectedIndex = 0;

                lvProducts.ItemsSource = _allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var productId = (int)((Button)sender).Tag;
            var product = _allProducts.FirstOrDefault(p => p.ProductId == productId);

            if (product == null) return;

            // Проверяем, есть ли достаточное количество товара на складе
            if (product.Quantity <= 0)
            {
                MessageBox.Show("Данный товар временно отсутствует в продаже.", "Нет в наличии", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var existingItem = AppConnect.model0db.Baskets
                .FirstOrDefault(b => b.UserId == _currentUser.UsersId && b.ProductId == productId);

            if (existingItem != null)
            {
                // Проверяем, хватает ли товара на складе, чтобы увеличить количество
                if (existingItem.Quantity + 1 > product.Quantity)
                {
                    MessageBox.Show($"Макс. возможное количество товара '{product.Name}' достигнуто ({product.Quantity}).", "Ограничение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                existingItem.Quantity++; // Увеличиваем количество в корзине
            }
            else
            {
                // Добавляем новую позицию в корзину
                var newItem = new Baskets
                {
                    ProductId = productId,
                    UserId = _currentUser.UsersId,
                    Quantity = 1
                };
                AppConnect.model0db.Baskets.Add(newItem);
            }

            try
            {
                AppConnect.model0db.SaveChanges();
                MessageBox.Show("Товар добавлен в корзину!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void CmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbFirms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void ApplyFilters()
        {
            var filtered = _allProducts.AsEnumerable();

            // Фильтр по категории (если выбрано не "По умолчанию")
            if (cmbCategories.SelectedIndex > 0 && cmbCategories.SelectedItem is Categories selectedCategory)
            {
                filtered = filtered.Where(p => p.CategoryId == selectedCategory.CategoryId);
            }

            // Фильтр по фирме (если выбрано не "По умолчанию")
            if (cmbFirms.SelectedIndex > 0 && cmbFirms.SelectedItem is Firms selectedFirm)
            {
                filtered = filtered.Where(p => p.FirmId == selectedFirm.FirmId);
            }

            // Фильтр по поиску
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                filtered = filtered.Where(p => p.Name.Contains(txtSearch.Text));
            }

            lvProducts.ItemsSource = filtered.ToList();
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CatalogPage(_currentUser));
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage(_currentUser));
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage(_currentUser));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }
    }
}