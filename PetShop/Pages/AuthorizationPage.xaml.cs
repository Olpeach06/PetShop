using PetShop.AppData;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class AuthorizationPage : Page
    {
        private string _lastEnteredEmail = string.Empty;

        public AuthorizationPage()
        {
            InitializeComponent();
            AppConnect.model0db = new PetShopOnEntities();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text;
            var password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                txtError.Text = "Введите email и пароль!";
                txtError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                var user = AppConnect.model0db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user == null)
                {
                    txtError.Text = "Неверный email или пароль!";
                    txtError.Visibility = Visibility.Visible;
                    return;
                }

                // Проверка роли пользователя
                if (user.RoleId == 1) // Администратор
                {
                    AppFrame.frameMain.Navigate(new AdminMainPage()); // Переход администратора на свою страницу
                }
                else // Обычный пользователь
                {
                    AppFrame.frameMain.Navigate(new CatalogPage(user)); // Переход пользователя на корзину
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}");
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            _lastEnteredEmail = txtEmail.Text; // Сохраняем email перед переходом
            AppFrame.frameMain.Navigate(new RegistrationPage(_lastEnteredEmail));
        }

        private void TxtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            _lastEnteredEmail = txtEmail.Text;
        }

        private void TxtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            // Восстанавливаем email, если он был введен
            if (!string.IsNullOrEmpty(_lastEnteredEmail))
            {
                txtEmail.Text = _lastEnteredEmail;
            }
            txtError.Visibility = Visibility.Collapsed;
        }
    }
}