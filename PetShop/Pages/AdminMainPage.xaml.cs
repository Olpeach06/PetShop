using PetShop.AppData;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class AdminMainPage : Page
    {
        private Frame _adminFrame;

        public AdminMainPage()
        {
            InitializeComponent();
            _adminFrame = this.FindName("AdminFrame") as Frame;

            // Добавляем проверку на null
            if (_adminFrame == null)
            {
                MessageBox.Show("Ошибка: не найден фрейм для навигации");
                return;
            }

            _adminFrame.Navigate(new AdminProductsPage(_adminFrame));
        }

        // Остальные методы остаются без изменений
        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            _adminFrame.Navigate(new AdminProductsPage(_adminFrame));
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            _adminFrame?.Navigate(new AdminOrdersPage());
        }

        //private void BtnUsers_Click(object sender, RoutedEventArgs e)
        //{
        //    // Временная заглушка, пока нет реализации AdminUsersPage
        //    MessageBox.Show("Раздел управления пользователями в разработке");

        //    // Если страница существует:
        //    // _adminFrame?.Navigate(new AdminUsersPage());
        //}

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.frameMain != null)
            {
                AppFrame.frameMain.Navigate(new AuthorizationPage());
            }
        }
    }
}