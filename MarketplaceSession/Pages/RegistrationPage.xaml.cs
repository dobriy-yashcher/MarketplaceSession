using MarketplaceSession.ADO;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MarketplaceSession.Pages
{
    /// <summary>
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
            CbRoles.ItemsSource = MarketplaceSessionEntities.GetContext().Role.ToList();
        }
        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// Проверка данных
            /// </summary>
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(TbLogin.Text))
                errors.AppendLine("Введите логин!");
            if (string.IsNullOrWhiteSpace(TbPassword.Password))
                errors.AppendLine("Введите пароль!");

            var findUser = MarketplaceSessionEntities.GetContext().User
                .Where(x => x.LogIn.Username == TbLogin.Text)
                .FirstOrDefault();
            if (findUser != null)
                errors.AppendLine("Данный логин уже занят!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Проверка соответсвия пароля к требованиям
            /// </summary>
            string input = TbPassword.Password;
            string pattern = @"^.*(?=\S{6,})(?=.*\p{Lu})(?=.*\d)(?=.*[!|@|#|$|%|^|.]).*$";
            var options = RegexOptions.CultureInvariant;

            var matches = Regex.Matches(input, pattern, options);

            if (matches.Count == 0)
            {
                MessageBox.Show("Пароль не соответсвует следющим требованиям:" + Environment.NewLine +
                    "Минимум 6 символов" + Environment.NewLine +
                    "Минимум 1 прописная буква." + Environment.NewLine +
                    "Минимум 1 цифра." + Environment.NewLine +
                    "Минимум один символ из набора: ! @ # $ % ^. ",
                    "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Добавление нового пользователя
            /// </summary>
            var login = new LogIn
            {
                Username = TbSurname.Text,
                Password = TbPassword.Password
            };
            MarketplaceSessionEntities.GetContext().LogIn.Add(login);

            var newUser = new User
            {
                Name = TbName.Text,
                Surname = TbSurname.Text,
                Role = (Role)CbRoles.SelectedItem,
                LogIn = login,
                Balance = 0,
            };

            try
            {
                MarketplaceSessionEntities.GetContext().User.Add(newUser);
                MarketplaceSessionEntities.GetContext().SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new AuthorizationPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
