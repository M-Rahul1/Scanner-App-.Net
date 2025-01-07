using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using App1.Models;
using System;

namespace App1.Views
{
    public partial class DetailsPage : ContentPage
    {
        private SQLiteAsyncConnection _database;
        private ObservableCollection<ScannedItem> _scannedItems;

        public DetailsPage()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScannedIDs.db3");
            _database = new SQLiteAsyncConnection(dbPath);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadScannedItemsAsync(); // Refresh data when the page appears
        }

        private async Task LoadScannedItemsAsync()
        {
            var items = await _database.Table<ScannedItem>().ToListAsync();
            _scannedItems = new ObservableCollection<ScannedItem>(items);
            ScannedIdsListView.ItemsSource = _scannedItems;
            UpdateCountLabel();
        }

        private void UpdateCountLabel()
        {
            DetailsCountLabel.Text = $"Count: {_scannedItems?.Count ?? 0}";
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
