using PetShop.AppData;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PetShop.Pages
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage(string email = null)
        {
            InitializeComponent();

            // Устанавливаем переданный email (если есть)
            if (!string.IsNullOrEmpty(email))
            {
                txtEmail.Text = email;
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password) ||
                    string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
                {
                    ShowError("Заполните все обязательные поля!");
                    return;
                }

                if (txtPassword.Password != txtConfirmPassword.Password)
                {
                    ShowError("Пароли не совпадают!");
                    return;
                }

                if (txtPassword.Password.Length < 6)
                {
                    ShowError("Пароль должен содержать минимум 6 символов!");
                    return;
                }

                if (!IsValidEmail(txtEmail.Text))
                {
                    ShowError("Введите корректный email адрес!");
                    return;
                }

                if (AppConnect.model0db.USERS.Any(u => u.email == txtEmail.Text))
                {
                    ShowError("Пользователь с таким email уже существует!");
                    return;
                }

                var newUser = new USERS
                {
                    last_name = txtLastName.Text.Trim(),
                    first_name = txtFirstName.Text.Trim(),
                    patronymic = string.IsNullOrWhiteSpace(txtPatronymic.Text) ? null : txtPatronymic.Text.Trim(),
                    email = txtEmail.Text.Trim(),
                    password = txtPassword.Password,
                    role_id = 2
                };

                AppConnect.model0db.USERS.Add(newUser);
                AppConnect.model0db.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                AppFrame.frameMain.Navigate(new AuthorizationPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new AuthorizationPage());
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}