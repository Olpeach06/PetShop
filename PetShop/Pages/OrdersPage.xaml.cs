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
        public OrdersPage(USERS user)
        {
            InitializeComponent();
            var orders = AppConnect.model0db.ZAKAZ
                .Where(z => z.users_id == user.users_id)
                .ToList();
            lvOrders.ItemsSource = orders;
        }
        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int orderId)
            {
                var order = AppConnect.model0db.ZAKAZ.Find(orderId);
                if (order != null)
                    NavigationService.Navigate(new OrderDetailsPage(order));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}