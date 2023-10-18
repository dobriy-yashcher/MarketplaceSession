using MarketplaceSession.ADO;
using MarketplaceSession.Components;
using MarketplaceSession.Properties;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MarketplaceSession.Pages
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var loginText = Settings.Default.LastUser;
            TbLogin.Text = loginText;

            if (loginText != "")
                ChbRemember.IsChecked = true;
        }

        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// Проверка блокировки авторизации
            /// </summary>
            if (Settings.Default.DateEndBlock > DateTime.Now)
            {
                MessageBox.Show("Многократный ввод неккоректных данных" + Environment.NewLine +
                    $"До разблокировки: {(Settings.Default.DateEndBlock - DateTime.Now).Seconds} секунд",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Проверка данных
            /// </summary>
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(TbLogin.Text))
                errors.AppendLine("Введите логин!");
            if (string.IsNullOrWhiteSpace(TbPassword.Password))
                errors.AppendLine("Введите пароль!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Обработка функции "Запомни меня"
            /// </summary>
            if (ChbRemember.IsChecked == true) Settings.Default.LastUser = TbLogin.Text;
            else Settings.Default.LastUser = "";

            Settings.Default.Save();

            /// <summary>
            /// Авторизация
            /// </summary>
            var findUser = MarketplaceSessionEntities
                .GetContext().User
                .Where(x => x.LogIn.Username == TbLogin.Text)
                .FirstOrDefault();

            if (findUser == null)
            {
                MessageBox.Show("Неверный логин!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (findUser.LogIn.Password != TbPassword.Password)
            {
                MessageBox.Show("Неверный пароль!", "Ошибка",
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
                MessageBox.Show("Авторизация прошла успешно!",
                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.Navigate(new MainPage());
            }
        }
    }
}
