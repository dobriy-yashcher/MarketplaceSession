using MarketplaceSession.ADO;
using MarketplaceSession.Components;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MarketplaceSession.Pages
{
    /// <summary>
    /// Interaction logic for CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        public int TotalPrice { get; set; }
        public ProductCart[] BasketEntries { get; set; }

        public CartPage()
        {
            Loaded += BasketPage_Loaded;
            InitializeComponent();
            UpdateList();
        }

        private void BasketPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void ReduceCountButton_Click(object sender, RoutedEventArgs e)
        {
            var basket = (ProductCart)((Button)sender).Tag;
            basket.Count--;
            if (basket.Count == 0)
            {
                MarketplaceSessionEntities.GetContext().ProductCart.Remove(basket);
                MarketplaceSessionEntities.GetContext().SaveChanges();
            }
            else
            {
                MarketplaceSessionEntities.GetContext().ProductCart.AddOrUpdate(basket);
                MarketplaceSessionEntities.GetContext().SaveChanges();
            }
            UpdateList();
        }

        private void IncreaseCountButton_Click(object sender, RoutedEventArgs e)
        {
            var basket = (ProductCart)((Button)sender).Tag;
            basket.Count++;
            MarketplaceSessionEntities.GetContext().ProductCart.AddOrUpdate(basket);
            MarketplaceSessionEntities.GetContext().SaveChanges();
            UpdateList();

        }

        private void UpdateList()
        {
            BasketEntries = MarketplaceSessionEntities.GetContext().ProductCart.Where(x => x.CartId == Manager.CurrentCart.Id).ToArray();
            TotalPrice = BasketEntries.Sum(x => x.Product.Cost * x.Count);
            TotalTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            MainListView.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!MarketplaceSessionEntities.GetContext().ProductCart.Any(x => x.CartId == Manager.CurrentCart.Id))
            {
                MessageBox.Show("The cart is empty!");
                return;
            }

            var order = new Order { OrderDate = DateTime.Now, StatusId = 1, CartId = Manager.CurrentCart.Id};
            MarketplaceSessionEntities.GetContext().Order.Add(order);
            MarketplaceSessionEntities.GetContext().SaveChanges();

            Manager.CurrentCart = new Cart { UserId = Manager.AuthorizedUser.Id};
            MarketplaceSessionEntities.GetContext().Cart.Add(Manager.CurrentCart);
            MarketplaceSessionEntities.GetContext().SaveChanges();

            NavigationService.Navigate(new OrderPage(order.Id));
            MessageBox.Show("The order has been successfully placed");
        }
    }
}
