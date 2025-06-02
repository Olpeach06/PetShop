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
using System.Data.Entity;

namespace PetShop.Pages
{
    public partial class OrderDetailsPage : Page
    {
        private Zakazy _currentOrder;

        public OrderDetailsPage(Zakazy order)
        {
            InitializeComponent();
            _currentOrder = order;
            DataContext = _currentOrder;
            LoadOrderItems();
            CalculateTotal();
        }

        private void LoadOrderItems()
        {
            var items = AppConnect.model0db.Purchases
                .Where(p => p.ZakazId == _currentOrder.ZakazId)
                .Include(p => p.Products) // Важно: загружаем связанный товар
                .ToList();

            lvItems.ItemsSource = items;
        }

        private void CalculateTotal()
        {
            decimal total = AppConnect.model0db.Purchases
                .Where(p => p.ZakazId == _currentOrder.ZakazId)
                .Sum(p => p.Quantity * p.Products.Price);

            txtTotal.Text = total.ToString("C");
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}