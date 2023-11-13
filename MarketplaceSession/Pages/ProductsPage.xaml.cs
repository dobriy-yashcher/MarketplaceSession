using ProductDelivery.ADO;
using ProductDelivery.Components;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace ProductDelivery.Pages
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        public Visibility AddToCartButtonVisibility { get; set; }

        private static readonly Category _allProductType = new Category() { Name = "All" };
        public static string[] Sortings { get; set; } = new[] { "Ascending prices", "Descending prices", "By default" };

        public Product[] Products { get; set; }
        public Category[] ProductTypes { get; set; }
        public int SelectedSortingIndex { get; set; } = 2;
        public Category SelectedType { get; set; } = _allProductType;
        public string SearchText { get; set; } = string.Empty;
        public int TotalItemsCount { get; set; }
        public int FilteredItemsCount { get; set; }

        public ProductsPage()
        {
            AddToCartButtonVisibility = Manager.AuthorizedUser.Role.Name == "Client" ? Visibility.Visible : Visibility.Hidden;
            var productTypes = ProductDeliveryEntities.GetContext().Category.ToArray();
            ProductTypes = productTypes.Concat(new[] { _allProductType }).ToArray();
            InitializeComponent();
        }


        private void UpdateSource()
        {
            Func<Product, object> sorting;
            switch (SelectedSortingIndex)
            {
                case 0:
                    sorting = x => x.Cost;
                    break;
                case 1:
                    sorting = x => -x.Cost;
                    break;
                default:
                    sorting = x => x.Id;
                    break;
            }

            Func<Product, bool> filter;

            if (SelectedType == _allProductType)
                filter = x => x.Title.ToLower().Contains(SearchText.ToLower());
            else
                filter = x => x.Title.ToLower().Contains(SearchText.ToLower()) && x.CategoryId == SelectedType.Id;

            var query = ProductDeliveryEntities.GetContext().Product.Where(x => !x.IsActual).ToArray();

            TotalItemsCount = query.Length;
            Products = query.Where(x => filter(x)).OrderBy(x => sorting(x)).ToArray();
            FilteredItemsCount = Products.Length;
            MainListView.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSource();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var product = (Product)((ListView)sender).SelectedItem;
            NavigationService.Navigate(new ProductPage(product.Id));
        }

        private void AddToBasketButton_Click(object sender, RoutedEventArgs e)
        {
            var product = (Product)((Button)sender).Tag;

            var basketEntry = ProductDeliveryEntities.GetContext().ProductCart
              .FirstOrDefault(x => x.CartId == Manager.CurrentCart.Id && x.ProductId == product.Id)
              ?? new ProductCart { ProductId = product.Id, CartId = Manager.CurrentCart.Id };

            basketEntry.Count++;

            ProductDeliveryEntities.GetContext().ProductCart.AddOrUpdate(basketEntry);
            ProductDeliveryEntities.GetContext().SaveChanges();

            MessageBox.Show($"The product has been added to the cart. Current quantity: {basketEntry.Count}", "Successfully",
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ApplySearchButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSource();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text) && txtSearch.Text.Length > 0)
                textSearch.Visibility = Visibility.Collapsed;
            else
                textSearch.Visibility = Visibility.Visible;
        }

        private void textSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearch.Focus();
        }
    }
}
