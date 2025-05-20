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
        private USERS _currentUser;
        private List<PRODUCTS> _allProducts;

        public CatalogPage(USERS user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _allProducts = AppConnect.model0db.PRODUCTS
                    .Include(p => p.CATEGORIES)
                    .Include(p => p.FIRM)
                    .Include(p => p.TYPE)
                    .ToList();

                // Загрузка категорий с пунктом "По умолчанию"
                var categories = AppConnect.model0db.CATEGORIES.ToList();
                cmbCategories.Items.Clear();
                cmbCategories.Items.Add(new { name = "по умолчанию", category_id = -1 }); // Первый элемент
                foreach (var cat in categories)
                    cmbCategories.Items.Add(cat);
                cmbCategories.SelectedIndex = 0;

                // Загрузка фирм с пунктом "По умолчанию"
                var firms = AppConnect.model0db.FIRM.ToList();
                cmbFirms.Items.Clear();
                cmbFirms.Items.Add(new { name = "по умолчанию", category_id = -1 }); // Первый элемент
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
            var product = _allProducts.FirstOrDefault(p => p.product_id == productId);

            if (product == null) return;

            var existingItem = AppConnect.model0db.BASKET
                .FirstOrDefault(b => b.users_id == _currentUser.users_id && b.product_id == productId);

            if (existingItem != null)
            {
                existingItem.quantity++;
            }
            else
            {
                var newItem = new BASKET
                {
                    product_id = productId,
                    users_id = _currentUser.users_id,
                    quantity = 1
                };
                AppConnect.model0db.BASKET.Add(newItem);
            }

            try
            {
                AppConnect.model0db.SaveChanges();
                MessageBox.Show("Товар добавлен в корзину!");
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
            if (cmbCategories.SelectedIndex > 0 && cmbCategories.SelectedItem is CATEGORIES selectedCategory)
            {
                filtered = filtered.Where(p => p.category_id == selectedCategory.category_id);
            }

            // Фильтр по фирме (если выбрано не "По умолчанию")
            if (cmbFirms.SelectedIndex > 0 && cmbFirms.SelectedItem is FIRM selectedFirm)
            {
                filtered = filtered.Where(p => p.firm_id == selectedFirm.firm_id);
            }

            // Фильтр по поиску
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                filtered = filtered.Where(p => p.name.Contains(txtSearch.Text));
            }

            lvProducts.ItemsSource = filtered.ToList();
        }
    }
}