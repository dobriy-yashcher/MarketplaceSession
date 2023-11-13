using MarketplaceSession.ADO;
using MarketplaceSession.Components;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MarketplaceSession.Pages
{
    /// <summary>
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public Order[] Orders { get; set; }
        public int TotalPrice { get; set; }

        public OrdersPage()
        {
            var fetchAll = Manager.AuthorizedUser.Role.Name == "Admin";
            if (fetchAll)
                Orders = MarketplaceSessionEntities.GetContext().Order.OrderByDescending(x => x.OrderDate).ToArray();
            else
                Orders = MarketplaceSessionEntities.GetContext().Order.Where(x => x.Cart.UserId == Manager.AuthorizedUser.Id).OrderByDescending(x => x.OrderDate).ToArray();

            //foreach (Order order in Orders)
            //{
            //    var products = MarketplaceSessionEntities.GetContext().ProductCart.Where(x => x.CartId == order.CartId).ToArray();
            //    TotalPrice += products.Sum(x => x.Count * x.Product.Cost);
            //}

            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var order = (Order)((ListView)sender).SelectedItem;
            NavigationService.Navigate(new OrderPage(order.Id));
        }
    }
}
