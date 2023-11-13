using ProductDelivery.Components;
using ProductDelivery.Pages;
using ProductDelivery.Windows;
using System.Windows;
using System.Windows.Input;

namespace ProductDelivery
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            Manager.MainFrame = MainFrame;
            MainFrame.Navigate(new AuthorizationPage());
        }

        private void MainFrame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
