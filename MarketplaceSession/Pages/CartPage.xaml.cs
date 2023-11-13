using ProductDelivery.ADO;
using ProductDelivery.Components;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ProductDelivery.Pages
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
                ProductDeliveryEntities.GetContext().ProductCart.Remove(basket);
                ProductDeliveryEntities.GetContext().SaveChanges();
            }
            else
            {
                ProductDeliveryEntities.GetContext().ProductCart.AddOrUpdate(basket);
                ProductDeliveryEntities.GetContext().SaveChanges();
            }
            UpdateList();
        }

        private void IncreaseCountButton_Click(object sender, RoutedEventArgs e)
        {
            var basket = (ProductCart)((Button)sender).Tag;
            basket.Count++;
            ProductDeliveryEntities.GetContext().ProductCart.AddOrUpdate(basket);
            ProductDeliveryEntities.GetContext().SaveChanges();
            UpdateList();

        }

        private void UpdateList()
        {
            BasketEntries = ProductDeliveryEntities.GetContext().ProductCart.Where(x => x.CartId == Manager.CurrentCart.Id).ToArray();
            TotalPrice = BasketEntries.Sum(x => x.Product.Cost * x.Count);
            TotalTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            MainListView.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ProductDeliveryEntities.GetContext().ProductCart.Any(x => x.CartId == Manager.CurrentCart.Id))
            {
                MessageBox.Show("The cart is empty!");
                return;
            }

            var order = new Order { OrderDate = DateTime.Now, StatusId = 1, CartId = Manager.CurrentCart.Id};
            ProductDeliveryEntities.GetContext().Order.Add(order);
            ProductDeliveryEntities.GetContext().SaveChanges();

            Manager.CurrentCart = new Cart { UserId = Manager.AuthorizedUser.Id};
            ProductDeliveryEntities.GetContext().Cart.Add(Manager.CurrentCart);
            ProductDeliveryEntities.GetContext().SaveChanges();

            NavigationService.Navigate(new OrderPage(order.Id));
            MessageBox.Show("The order has been successfully placed");
        }
    }
}
