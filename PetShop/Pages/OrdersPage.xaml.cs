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
using PetShop.AppData;

namespace PetShop.Pages
{
    public partial class OrdersPage : Page
    {
        private Users _currentUser;

        public OrdersPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;

            var orders = AppConnect.model0db.Zakazy
                .Where(z => z.UserId == user.UsersId)
                .ToList();
            lvOrders.ItemsSource = orders;
        }

        // Добавляем недостающие методы обработки кнопок
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
            // Уже находимся на странице заказов, можно обновить данные
            var orders = AppConnect.model0db.Zakazy
                .Where(z => z.UserId == _currentUser.UsersId)
                .ToList();
            lvOrders.ItemsSource = orders;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int orderId)
            {
                var order = AppConnect.model0db.Zakazy.Find(orderId);
                if (order != null) { 
                    NavigationService.Navigate(new OrderDetailsPage(order)); }
                else { MessageBox.Show("sdfdsfds"); }
            }
            else { MessageBox.Show("111111111"); }
        }
    }
}