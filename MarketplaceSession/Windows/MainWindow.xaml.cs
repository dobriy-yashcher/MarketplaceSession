using ProductDelivery.Components;
using ProductDelivery.Pages;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace ProductDelivery.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CreateProductButton.Visibility = Manager.AuthorizedUser.Role.Name == "Admin" ? Visibility.Visible : Visibility.Collapsed;
            BasketButton.Visibility = Manager.AuthorizedUser.Role.Name == "Client" ? Visibility.Visible : Visibility.Collapsed;
            Manager.MainFrame = MainFrame;
            MainFrame.Navigate(new ProductsPage());
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new OrdersPage());
        }

        private void CreateProudctButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage());
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CartPage());
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }
    }
}
