using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Product> Products { get; set; }
        private Product selectedProduct;
        private bool isNameAscending = true;
        private bool isDateAscending = true;
        private string lastUsedLocation;
        private DateTime lastUsedDate;

        public MainPage()
        {
            InitializeComponent();
            Products = new ObservableCollection<Product>();
            ProductsCollectionView.ItemsSource = Products;
            lastUsedDate = DateTime.Now;
        }

        private void OnSaveProductClicked(object sender, EventArgs e)
        {
            var product = new Product
            {
                Name = ProductEntry.Text,
                Location = string.IsNullOrWhiteSpace(LocationEntry.Text) ? null : LocationEntry.Text,
                ExpiryDate = ExpiryDatePicker.Date
            };

            Products.Add(product);

            ProductEntry.Text = string.Empty;
            lastUsedLocation = LocationEntry.Text = string.IsNullOrWhiteSpace(LocationEntry.Text) ? lastUsedLocation : LocationEntry.Text;
            lastUsedDate = ExpiryDatePicker.Date;

            UpdateHashString();
        }

        private void OnEditProductClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var product = button.BindingContext as Product;
            selectedProduct = product;

            EditProductEntry.Text = product.Name;
            EditLocationEntry.Text = product.Location == null ? string.Empty : product.Location;
            EditExpiryDatePicker.Date = product.ExpiryDate;

            EditSection.IsVisible = true;
        }

        private void OnDeleteProductClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var product = button.BindingContext as Product;

            Products.Remove(product);

            UpdateHashString();
        }

        private void OnSaveChangesClicked(object sender, EventArgs e)
        {
            if (selectedProduct != null)
            {
                selectedProduct.Name = EditProductEntry.Text;
                selectedProduct.Location = string.IsNullOrWhiteSpace(EditLocationEntry.Text) ? null : EditLocationEntry.Text;
                selectedProduct.ExpiryDate = EditExpiryDatePicker.Date;

                ProductsCollectionView.ItemsSource = null;
                ProductsCollectionView.ItemsSource = Products;

                EditSection.IsVisible = false;

                UpdateHashString();
            }
        }

        private void OnCancelEditClicked(object sender, EventArgs e)
        {
            EditSection.IsVisible = false;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                ProductsCollectionView.SelectedItem = null;
            }
        }

        private void OnSortByNameTapped(object sender, EventArgs e)
        {
            var sortedProducts = isNameAscending
                ? new ObservableCollection<Product>(Products.OrderBy(p => p.Name))
                : new ObservableCollection<Product>(Products.OrderByDescending(p => p.Name));

            isNameAscending = !isNameAscending;

            Products.Clear();
            foreach (var product in sortedProducts)
            {
                Products.Add(product);
            }
        }

        private void OnSortByDateTapped(object sender, EventArgs e)
        {
            var sortedProducts = isDateAscending
                ? new ObservableCollection<Product>(Products.OrderBy(p => p.ExpiryDate))
                : new ObservableCollection<Product>(Products.OrderByDescending(p => p.ExpiryDate));

            isDateAscending = !isDateAscending;

            Products.Clear();
            foreach (var product in sortedProducts)
            {
                Products.Add(product);
            }
        }

        private void OnSortByLocationTapped(object sender, EventArgs e)
        {
            var sortedProducts = Products.OrderBy(p => p.Location).ToList();
            Products.Clear();
            foreach (var product in sortedProducts)
            {
                Products.Add(product);
            }
        }

        private void OnFilterLocationTextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = e.NewTextValue;

            if (string.IsNullOrWhiteSpace(filter))
            {
                ProductsCollectionView.ItemsSource = Products;
            }
            else
            {
                ProductsCollectionView.ItemsSource = Products.Where(p => p.Location != null && p.Location.Contains(filter)).ToList();
            }
        }

        private void OnHashStringEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(Encoding.UTF8.GetString(Convert.FromBase64String(e.NewTextValue)));
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
                HashErrorLabel.IsVisible = false;
            }
            catch
            {
                HashErrorLabel.IsVisible = true;
            }
        }

        private void UpdateHashString()
        {
            var json = JsonConvert.SerializeObject(Products);
            var hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            HashStringEntry.Text = hash;
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string Details => $"Location: {Location}, Expiry Date: {ExpiryDate:d}";
    }
}
