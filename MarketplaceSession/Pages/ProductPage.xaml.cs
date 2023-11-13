using ProductDelivery.ADO;
using ProductDelivery.Components;
using Microsoft.Win32;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ProductDelivery.Pages
{
    /// <summary>
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private static readonly Regex _numberRegex = new Regex("[^0-9]+");
        public Product Product { get; set; } = new Product();

        private bool _isReadonly = true;
        private bool _isNew = true;

        public Category[] ProductTypes { get; set; }
        public bool IsReadonly => _isReadonly;
        public bool IsNotReadonly => !_isReadonly;
        public Visibility AdminButtonsVisibility => _isReadonly ? Visibility.Hidden : Visibility.Visible;
        public Visibility DeleteButtonVisibility => _isReadonly || _isNew ? Visibility.Hidden : Visibility.Visible;

        public ProductPage(int? productId = null)
        {
            if (productId != null)
            {
                _isNew = false;
                Product = ProductDeliveryEntities.GetContext().Product.Find(productId);
            }
            _isReadonly = Manager.AuthorizedUser.Role.Name == "Client";
            ProductTypes = ProductDeliveryEntities.GetContext().Category.ToArray();
            Unloaded += ProductPage_Unloaded;

            InitializeComponent();

            AddToCartButton.Visibility = Manager.AuthorizedUser.Role.Name == "Admin" ? Visibility.Hidden : Visibility.Visible;
        }

        private void ProductPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Product.Id != 0)
                ProductDeliveryEntities.GetContext().Entry(Product).Reload();
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Product.Title))
            {
                MessageBox.Show("Name not entered");
                return;
            }

            if (Product.Cost == 0)
            {
                MessageBox.Show("The cost is not entered");
                return;
            }

            if (Product.Category == null)
            {
                MessageBox.Show("Category not entered");
                return;
            }

            if (Product.Image == null)
            {
                MessageBox.Show("Image not selected");
                return;
            }

            ProductDeliveryEntities.GetContext().Product.AddOrUpdate(Product);
            ProductDeliveryEntities.GetContext().SaveChanges();
            NavigationService.GoBack();
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberRegex.IsMatch(e.Text);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure?", "Confirmation of deletion",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                Product.IsActual = true;
                ProductDeliveryEntities.GetContext().Product.AddOrUpdate(Product);
                ProductDeliveryEntities.GetContext().SaveChanges();
                NavigationService.GoBack();
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new OpenFileDialog();
            if (window.ShowDialog() == true)
            {
                using (MemoryStream ms = new MemoryStream())
                using (Stream fileStream = window.OpenFile())
                {
                    fileStream.CopyTo(ms);
                    Product.Image = ms.ToArray();
                    MainImage.ImageSource = GenerateImageSource(ms);
                }
            }
        }

        private ImageSource GenerateImageSource(MemoryStream file)
        {
            file.Position = 0;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = file;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var basketEntry = ProductDeliveryEntities.GetContext().ProductCart
                     .FirstOrDefault(x => x.CartId == Manager.CurrentCart.Id && x.ProductId == Product.Id)
                     ?? new ProductCart { ProductId = Product.Id, CartId = Manager.CurrentCart.Id };

            basketEntry.Count++;

            ProductDeliveryEntities.GetContext().ProductCart.AddOrUpdate(basketEntry);
            ProductDeliveryEntities.GetContext().SaveChanges();

            MessageBox.Show($"The product has been added to the cart. Current quantity: {basketEntry.Count}", "Successfully",
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text) && txtName.Text.Length > 0)
                textName.Visibility = Visibility.Collapsed;
            else
                textName.Visibility = Visibility.Visible;
        }

        private void textName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Focus();
        }

        private void textCost_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtCost.Focus();
        }

        private void txtCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCost.Text) && txtCost.Text.Length > 0)
                textCost.Visibility = Visibility.Collapsed;
            else
                textCost.Visibility = Visibility.Visible;
        }
    }
}
