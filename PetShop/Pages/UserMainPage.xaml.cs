using PetShop.AppData;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class UserMainPage : Page
    {
        private USERS _currentUser;
        private Frame _userFrame; // Добавляем поле для фрейма

        public UserMainPage(USERS user, Frame userFrame) // Добавляем параметр
        {
            InitializeComponent();
            _currentUser = user;
            _userFrame = userFrame;
            _userFrame.Navigate(new CatalogPage(_currentUser));
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            _userFrame.Navigate(new CatalogPage(_currentUser));
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            _userFrame.Navigate(new CartPage(_currentUser));
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            _userFrame.Navigate(new OrdersPage(_currentUser));
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AuthorizationPage());
        }
    }
}