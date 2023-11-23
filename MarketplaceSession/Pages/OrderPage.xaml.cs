using ProductDelivery.ADO;
using ProductDelivery.Components;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace ProductDelivery.Pages
{
    /// <summary>
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public Visibility AdminButtonsVisible { get; set; } = Manager.AuthorizedUser.Role.Name == "Admin" ? Visibility.Visible : Visibility.Hidden;

        public Status[] Statuses { get; set; }
        public Status SelectedStatus { get; set; }
        public Order Order { get; set; }
        public ProductCart[] Products { get; set; }
        public int OrderSum { get; set; }

        public OrderPage(int orderId)
        {
            Order = ProductDeliveryEntities.GetContext().Order.Find(orderId);
            SelectedStatus = Order.Status;
            Products = ProductDeliveryEntities.GetContext().ProductCart.Where(x => x.CartId == Order.CartId).ToArray();
            Statuses = ProductDeliveryEntities.GetContext().Status.ToArray();
            OrderSum = Products.Sum(x => x.Count * x.Product.Cost);

            InitializeComponent();

            Loaded += OrderPage_Loaded;

            if (Manager.AuthorizedUser.Role.Name == "Client")
                cbStatus.IsEnabled = false;
        }

        private void OrderPage_Loaded(object sender, RoutedEventArgs e)
        {           
            var toggleButton = (ToggleButton)cbStatus.Template.FindName("toggleButton", cbStatus);
            if (toggleButton != null)
            {
                var border = (Border)toggleButton.Template.FindName("templateRoot", toggleButton);
                if (border != null)
                {
                    border.Background = System.Windows.Media.Brushes.Transparent;
                }
            }    
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var product = (ProductCart)((ListView)sender).SelectedItem;
            NavigationService.Navigate(new ProductPage(product.Product.Id));
        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            Order.StatusId = SelectedStatus.Id;

            ProductDeliveryEntities.GetContext().Order.AddOrUpdate(Order);
            ProductDeliveryEntities.GetContext().SaveChanges();

            MessageBox.Show($"Status changed", "Successfully",
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}
