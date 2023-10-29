using MarketplaceSession.ADO;
using MarketplaceSession.Components;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            //CbRoles.ItemsSource = MarketplaceSessionEntities.GetContext().Role.ToList();
        }

        //private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        //{
        //    /// <summary>
        //    /// Проверка данных
        //    /// </summary>
        //    StringBuilder errors = new StringBuilder();

        //    if (string.IsNullOrWhiteSpace(TbLogin.Text))
        //        errors.AppendLine("Введите логин!");
        //    if (string.IsNullOrWhiteSpace(TbPassword.Password))
        //        errors.AppendLine("Введите пароль!");

        //    var findUser = MarketplaceSessionEntities.GetContext().User
        //        .Where(x => x.LogIn.Username == TbLogin.Text)
        //        .FirstOrDefault();
        //    if (findUser != null)
        //        errors.AppendLine("Данный логин уже занят!");

        //    if (errors.Length > 0)
        //    {
        //        MessageBox.Show(errors.ToString(), "Ошибка",
        //            MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }

        //    /// <summary>
        //    /// Проверка соответсвия пароля к требованиям
        //    /// </summary>
        //    string input = TbPassword.Password;
        //    string pattern = @"^.*(?=\S{6,})(?=.*\p{Lu})(?=.*\d)(?=.*[!|@|#|$|%|^|.]).*$";
        //    var options = RegexOptions.CultureInvariant;

        //    var matches = Regex.Matches(input, pattern, options);

        //    if (matches.Count == 0)
        //    {
        //        MessageBox.Show("Пароль не соответсвует следющим требованиям:" + Environment.NewLine +
        //            "Минимум 6 символов" + Environment.NewLine +
        //            "Минимум 1 прописная буква." + Environment.NewLine +
        //            "Минимум 1 цифра." + Environment.NewLine +
        //            "Минимум один символ из набора: ! @ # $ % ^. ",
        //            "Ошибка",
        //            MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }

        //    /// <summary>
        //    /// Добавление нового пользователя
        //    /// </summary>
        //    var login = new LogIn
        //    {
        //        Username = TbSurname.Text,
        //        Password = TbPassword.Password
        //    };
        //    MarketplaceSessionEntities.GetContext().LogIn.Add(login);

        //    var newUser = new User
        //    {
        //        Name = TbName.Text,
        //        Surname = TbSurname.Text,
        //        Role = (Role)CbRoles.SelectedItem,
        //        LogIn = login,
        //        Balance = 0,
        //    };

        //    try
        //    {
        //        MarketplaceSessionEntities.GetContext().User.Add(newUser);
        //        MarketplaceSessionEntities.GetContext().SaveChanges();

        //        MessageBox.Show("Регистрация прошла успешно!", "Успешно",
        //            MessageBoxButton.OK, MessageBoxImage.Information);
        //        NavigationService.Navigate(new AuthorizationPage());
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message.ToString(), "Ошибка",
        //            MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AuthorizationPage());
        }

        #region MouseDown
        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Focus();
        }     

        private void textSurname_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSurname.Focus();
        }
               
        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus();
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Focus();
        }
        #endregion

        #region Changed
        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text) && txtName.Text.Length > 0)
                textName.Visibility = Visibility.Collapsed;
            else
                textName.Visibility = Visibility.Visible;
        }          
           
        private void txtSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSurname.Text) && txtSurname.Text.Length > 0)
                textSurname.Visibility = Visibility.Collapsed;
            else
                textSurname.Visibility = Visibility.Visible;
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
        #endregion

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// Проверка данных
            /// </summary>
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(txtName.Text))
                errors.AppendLine("Enter your name!");
            if (string.IsNullOrWhiteSpace(txtSurname.Text))
                errors.AppendLine("Enter your surname!");
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                errors.AppendLine("Enter your username!");
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
                errors.AppendLine("Enter the password!");

            var findUser = MarketplaceSessionEntities.GetContext().User
                .Where(x => x.LogIn.Username == txtUsername.Text)
                .FirstOrDefault();
            if (findUser != null)
                errors.AppendLine("This login is already occupied!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Проверка соответсвия пароля к требованиям
            /// </summary>
            string input = passwordBox.Password;
            string pattern = @"^.*(?=\S{6,})(?=.*\p{Lu})(?=.*\d)(?=.*[!|@|#|$|%|^|.]).*$";
            var options = RegexOptions.CultureInvariant;

            var matches = Regex.Matches(input, pattern, options);

            if (matches.Count == 0)
            {
                MessageBox.Show("The password does not meet the following requirements:" + Environment.NewLine +
                    "Minimum of 6 characters." + Environment.NewLine +
                    "Minimum 1 uppercase letter." + Environment.NewLine +
                    "Minimum 1 number." + Environment.NewLine +
                    "Minimum one character from the set: ! @ # $ % ^. ",
                    "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /// <summary>
            /// Добавление нового пользователя
            /// </summary>
            var login = new LogIn
            {
                Username = txtUsername.Text,
                Password = passwordBox.Password
            };
            MarketplaceSessionEntities.GetContext().LogIn.Add(login);

            var newUser = new User
            {
                Name = txtName.Text,
                Surname = txtUsername.Text,
                RoleId = 1, //client
                LogIn = login,
            };

            try
            {
                MarketplaceSessionEntities.GetContext().User.Add(newUser);
                MarketplaceSessionEntities.GetContext().SaveChanges();

                MessageBox.Show("Registration was successful!", "Successfully",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new AuthorizationPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }                        
}
