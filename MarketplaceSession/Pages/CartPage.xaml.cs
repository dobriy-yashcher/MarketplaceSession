using Emgu.CV.Features2D;
using MarketplaceSession.Components;
using MarketplaceSession.Windows;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Microsoft.Win32;
using ProductDelivery.ADO;
using ProductDelivery.Components;
using System;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Drawing.Imaging;
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
        QRCodeEncoder encoder = new QRCodeEncoder();
        QRCodeDecoder decoder = new QRCodeDecoder();
        SaveFileDialog saveFile = new SaveFileDialog();
        OpenFileDialog openFileDialog = new OpenFileDialog();
        Bitmap bitmap;

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
                MessageBox.Show("The cart is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var order = new Order { OrderDate = DateTime.Now, StatusId = 1, CartId = Manager.CurrentCart.Id };
            ProductDeliveryEntities.GetContext().Order.Add(order);
            ProductDeliveryEntities.GetContext().SaveChanges();

            Manager.CurrentCart = new Cart { UserId = Manager.AuthorizedUser.Id };
            ProductDeliveryEntities.GetContext().Cart.Add(Manager.CurrentCart);
            ProductDeliveryEntities.GetContext().SaveChanges();

            NavigationService.Navigate(new OrderPage(order.Id));
            MessageBox.Show("The order has been successfully placed", "Successfully",
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ShareCart_Click(object sender, RoutedEventArgs e)
        {
            // create qr
            string qrtext = Manager.CurrentCart.Id.ToString();

            encoder.QRCodeScale = 8;
            bitmap = encoder.Encode(qrtext);

            // save qr
            saveFile.Filter = "PNG|*.png";
            saveFile.FileName = "C:/Users/dobriy_yashcher/Desktop/" +
                $"cart_{Manager.AuthorizedUser.Surname}{Manager.AuthorizedUser.Name}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}_{DateTime.Now.TimeOfDay.TotalSeconds}.png";
            if (bitmap != null)
            {
                bitmap.Save(saveFile.FileName, ImageFormat.Png);

                Manager.CurrentCart = new Cart { UserId = Manager.AuthorizedUser.Id };
                ProductDeliveryEntities.GetContext().Cart.Add(Manager.CurrentCart);
                ProductDeliveryEntities.GetContext().SaveChanges();
                UpdateList();

                MessageBox.Show("QRCode has been saved on the desktop", "Successfully", 
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void ImportCart_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                Manager.CurrentCart = new Cart { UserId = Manager.AuthorizedUser.Id };
                ProductDeliveryEntities.GetContext().Cart.Add(Manager.CurrentCart);
                ProductDeliveryEntities.GetContext().SaveChanges();

                try
                {
                    var qrtext = decoder.Decode(new QRCodeBitmapImage(new Bitmap(openFileDialog.FileName)));
                    var cartId = int.Parse(qrtext);

                    var products = ProductDeliveryEntities.GetContext().ProductCart.Where(x => x.CartId == cartId).ToArray();
                    foreach (var product in products)
                    {
                        var basketEntry = new ProductCart { ProductId = product.ProductId, CartId = Manager.CurrentCart.Id, Count = product.Count };

                        ProductDeliveryEntities.GetContext().ProductCart.AddOrUpdate(basketEntry);
                        ProductDeliveryEntities.GetContext().SaveChanges();
                    }

                    UpdateList();
                }
                catch
                {
                    MessageBox.Show("Invalid QRCode", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    Manager.CurrentCart = new Cart { UserId = Manager.AuthorizedUser.Id };
                    ProductDeliveryEntities.GetContext().Cart.Add(Manager.CurrentCart);
                    ProductDeliveryEntities.GetContext().SaveChanges();
                }
            }
        }

        private void ScanCart_Click(object sender, RoutedEventArgs e)
        {
            Manager.CurrentWindow.IsEnabled = false;

            var window = new QRScanWindow();
            window.Show();
        }
    }
}
