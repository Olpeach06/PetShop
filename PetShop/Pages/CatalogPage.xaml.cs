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
            _allProducts = AppConnect.model0db.PRODUCTS
                .Include(p => p.CATEGORIES)
                .Include(p => p.FIRM)
                .Include(p => p.TYPE)
                .ToList();

            lvProducts.ItemsSource = _allProducts;

            cmbCategories.ItemsSource = AppConnect.model0db.CATEGORIES.ToList();
            cmbCategories.SelectedIndex = -1;

            cmbFirms.ItemsSource = AppConnect.model0db.FIRM.ToList();
            cmbFirms.SelectedIndex = -1;
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
            var products = _allProducts.AsQueryable();

            if (cmbCategories.SelectedItem != null)
            {
                var category = (CATEGORIES)cmbCategories.SelectedItem;
                products = products.Where(p => p.category_id == category.category_id);
            }

            if (cmbFirms.SelectedItem != null)
            {
                var firm = (FIRM)cmbFirms.SelectedItem;
                products = products.Where(p => p.firm_id == firm.firm_id);
            }

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                products = products.Where(p => p.name.Contains(txtSearch.Text));
            }

            lvProducts.ItemsSource = products.ToList();
        }
    }
}