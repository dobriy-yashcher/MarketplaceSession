using MarketplaceSession.Components;
using MarketplaceSession.Pages;
using System.Windows;

namespace MarketplaceSession
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Manager.MainFrame = MainFrame;
            MainFrame.Navigate(new AuthorizationPage());
        }
    }
}
