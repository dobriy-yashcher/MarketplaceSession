﻿using MarketplaceSession.ADO;
using MarketplaceSession.Components;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MarketplaceSession.Pages
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
            Order = MarketplaceSessionEntities.GetContext().Order.Find(orderId);
            SelectedStatus = Order.Status;
            Products = MarketplaceSessionEntities.GetContext().ProductCart.Where(x => x.CartId == Order.CartId).ToArray();
            Statuses = MarketplaceSessionEntities.GetContext().Status.ToArray();
            OrderSum = Products.Sum(x => x.Count * x.Product.Cost);

            InitializeComponent();

            if (Manager.AuthorizedUser.Role.Name == "Client")
                cbStatus.IsEnabled = false;
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var product = (ProductCart)((ListView)sender).SelectedItem;
            NavigationService.Navigate(new ProductPage(product.Product.Id));
        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            Order.StatusId = SelectedStatus.Id;

            MarketplaceSessionEntities.GetContext().Order.AddOrUpdate(Order);
            MarketplaceSessionEntities.GetContext().SaveChanges();

            MessageBox.Show($"Status changed", "Successfully",
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}