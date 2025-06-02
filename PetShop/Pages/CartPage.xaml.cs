using PetShop.AppData;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class CartPage : Page
    {
        private Users _currentUser;

        public CartPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            var cartItems = AppConnect.model0db.Baskets
                .Where(b => b.UserId == _currentUser.UsersId)
                .Include(b => b.Products)
                .ToList();

            lvCartItems.ItemsSource = cartItems;

            decimal total = cartItems.Sum(item => item.Quantity * item.Products.Price);
            txtTotal.Text = $"Итого: {total:C}";
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var basketId = (int)((Button)sender).Tag;
            var itemToRemove = AppConnect.model0db.Baskets.Find(basketId);

            if (itemToRemove != null)
            {
                AppConnect.model0db.Baskets.Remove(itemToRemove);
                AppConnect.model0db.SaveChanges();
                LoadCartItems();
            }
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            var cartItems = AppConnect.model0db.Baskets
                .Where(b => b.UserId == _currentUser.UsersId)
                .ToList();

            if (!cartItems.Any())
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            try
            {
                // Создаем заказ
                var newOrder = new Zakazy
                {
                    UserId = _currentUser.UsersId,
                    Date = DateTime.Now,
                    StatusId = 1 // "В обработке"
                };

                AppConnect.model0db.Zakazy.Add(newOrder);
                AppConnect.model0db.SaveChanges();

                // Добавляем товары в заказ
                foreach (var item in cartItems)
                {
                    var purchase = new Purchases
                    {
                        ZakazId = newOrder.ZakazId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    AppConnect.model0db.Purchases.Add(purchase);

                    // Уменьшаем количество товара на складе
                    var product = AppConnect.model0db.Products.Find(item.ProductId);
                    product.Quantity -= item.Quantity;
                }

                // Очищаем корзину
                AppConnect.model0db.Baskets.RemoveRange(cartItems);
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
            var item = AppConnect.model0db.Baskets.Include(b => b.Products).FirstOrDefault(b => b.BasketId == basketId);

            if (item != null)
            {
                // Ограничиваем увеличение количества имеющимся запасом товара
                if (item.Quantity < item.Products.Quantity)
                {
                    item.Quantity++;
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
            var item = AppConnect.model0db.Baskets.Find(basketId);

            if (item != null && item.Quantity > 1)
            {
                // Уменьшаем количество товара на единицу
                item.Quantity--;
                AppConnect.model0db.SaveChanges();
                LoadCartItems(); // Обновляем содержимое корзины
            }
        }
    }
}