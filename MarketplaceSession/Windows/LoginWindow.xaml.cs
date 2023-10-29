using MarketplaceSession.Components;
using MarketplaceSession.Pages;
using MarketplaceSession.Windows;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceSession
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
