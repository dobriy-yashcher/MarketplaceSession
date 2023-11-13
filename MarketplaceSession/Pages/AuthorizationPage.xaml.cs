using ProductDelivery.ADO;
using ProductDelivery.Components;
using ProductDelivery.Pages;
using ProductDelivery.Properties;
using ProductDelivery.Windows;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProductDelivery.Pages
{
    /// <summary>
    /// Interaction logic for AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        const int MaxIncorrectPasswordInputCount = 3;
        const int BlockInputTimeInMinutes = 1;
        int _incorrectPasswordInputCount;

        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// Проверка блокировки авторизации
            /// </summary>
            if (Settings.Default.DateEndBlock > DateTime.Now)
            {
                MessageBox.Show("Multiple input of non-correct data" + Environment.NewLine +
                    $"Before unblocking: {(Settings.Default.DateEndBlock - DateTime.Now).Seconds} seconds",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Проверка данных
            /// </summary>
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                errors.AppendLine("Enter your username!");
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
                errors.AppendLine("Enter the password!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Обработка функции "Запомни меня"
            /// </summary>
            if (ChbRemember.IsChecked == true) Settings.Default.LastUser = txtUsername.Text;
            else Settings.Default.LastUser = "";

            Settings.Default.Save();

            /// <summary>
            /// Авторизация
            /// </summary>
            var findUser = ProductDeliveryEntities
                .GetContext().User
                .Where(x => x.LogIn.Username == txtUsername.Text)
                .FirstOrDefault();

            if (findUser == null)
            {
                MessageBox.Show("Invalid login!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (findUser.LogIn.Password != passwordBox.Password)
            {
                MessageBox.Show("Invalid password!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                _incorrectPasswordInputCount++;

                if (_incorrectPasswordInputCount >= MaxIncorrectPasswordInputCount)
                {
                    Settings.Default.DateEndBlock = DateTime.Now.AddMinutes(BlockInputTimeInMinutes);
                    Settings.Default.Save();
                    _incorrectPasswordInputCount = 0;
                }
            }
            else
            {
                MessageBox.Show("Authorization was successful!",
                    "Successfully", MessageBoxButton.OK, MessageBoxImage.Information);

                Manager.AuthorizedUser = findUser;

                Manager.CurrentCart = new Cart{ UserId = findUser.Id };
                ProductDeliveryEntities.GetContext().Cart.Add(Manager.CurrentCart);
                ProductDeliveryEntities.GetContext().SaveChanges();

                var mainView = new MainWindow();
                mainView.Show();
                Application.Current.Windows[0].Close();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var loginText = Settings.Default.LastUser;
            txtUsername.Text = loginText;

            if (loginText != "")
                ChbRemember.IsChecked = true;
        }

        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus();
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Focus();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && txtUsername.Text.Length > 0)
                textUsername.Visibility = Visibility.Collapsed;
            else
                textUsername.Visibility = Visibility.Visible;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(passwordBox.Password) && passwordBox.Password.Length > 0)
                textPassword.Visibility = Visibility.Collapsed;
            else
                textPassword.Visibility = Visibility.Visible;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new RegistrationPage());
        }
    }
}
