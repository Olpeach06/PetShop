﻿using PetShop.AppData;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class CartPage : Page
    {
        private USERS _currentUser;

        public CartPage(USERS user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            var cartItems = AppConnect.model0db.BASKET
                .Where(b => b.users_id == _currentUser.users_id)
                .Include(b => b.PRODUCTS)
                .ToList();

            lvCartItems.ItemsSource = cartItems;

            decimal total = cartItems.Sum(item => item.quantity * item.PRODUCTS.price);
            txtTotal.Text = $"Итого: {total:C}";
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var basketId = (int)((Button)sender).Tag;
            var itemToRemove = AppConnect.model0db.BASKET.Find(basketId);

            if (itemToRemove != null)
            {
                AppConnect.model0db.BASKET.Remove(itemToRemove);
                AppConnect.model0db.SaveChanges();
                LoadCartItems();
            }
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            var cartItems = AppConnect.model0db.BASKET
                .Where(b => b.users_id == _currentUser.users_id)
                .ToList();

            if (!cartItems.Any())
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            try
            {
                // Создаем заказ
                var newOrder = new ZAKAZ
                {
                    users_id = _currentUser.users_id,
                    date = DateTime.Now,
                    status_id = 1 // "В обработке"
                };

                AppConnect.model0db.ZAKAZ.Add(newOrder);
                AppConnect.model0db.SaveChanges();

                // Добавляем товары в заказ
                foreach (var item in cartItems)
                {
                    var purchase = new PURCHASE
                    {
                        zakaz_id = newOrder.zakaz_id,
                        product_id = item.product_id,
                        quantity = item.quantity
                    };
                    AppConnect.model0db.PURCHASE.Add(purchase);

                    // Уменьшаем количество товара на складе
                    var product = AppConnect.model0db.PRODUCTS.Find(item.product_id);
                    product.quantity -= item.quantity;
                }

                // Очищаем корзину
                AppConnect.model0db.BASKET.RemoveRange(cartItems);
                AppConnect.model0db.SaveChanges();

                MessageBox.Show("Заказ успешно оформлен!");
                LoadCartItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CatalogPage(_currentUser));
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            // Уже находимся в корзине, просто обновляем данные
            LoadCartItems();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage(_currentUser));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        // Увеличение количества товара
        private void IncreaseQty_Click(object sender, RoutedEventArgs e)
        {
            var basketId = (int)((Button)sender).Tag;
            var item = AppConnect.model0db.BASKET.Include(b => b.PRODUCTS).FirstOrDefault(b => b.basket_id == basketId);

            if (item != null)
            {
                // Ограничиваем увеличение количества имеющимся запасом товара
                if (item.quantity < item.PRODUCTS.quantity)
                {
                    item.quantity++;
                    AppConnect.model0db.SaveChanges();
                    LoadCartItems(); // Обновляем содержимое корзины
                }
                else
                {
                    MessageBox.Show("Нельзя добавить больше товара, чем есть в наличии!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // Уменьшение количества товара
        private void DecreaseQty_Click(object sender, RoutedEventArgs e)
        {
            var basketId = (int)((Button)sender).Tag;
            var item = AppConnect.model0db.BASKET.Find(basketId);

            if (item != null && item.quantity > 1)
            {
                // Уменьшаем количество товара на единицу
                item.quantity--;
                AppConnect.model0db.SaveChanges();
                LoadCartItems(); // Обновляем содержимое корзины
            }
        }
    }
}