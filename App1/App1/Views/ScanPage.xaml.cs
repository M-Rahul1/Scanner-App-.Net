using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using SQLite;
using Xamarin.Forms;
using App1.Models;

namespace App1.Views
{
    public partial class ScanPage : ContentPage
    {
        private SQLiteAsyncConnection _database;
        private ObservableCollection<string> _scannedIds;

        public ScanPage()
        {
            InitializeComponent();
            InitializeDatabase();
            _scannedIds = new ObservableCollection<string>();
            IdListView.ItemsSource = _scannedIds;
            UpdateCountLabel();
        }

        private async void InitializeDatabase()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScannedIDs.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<ScannedItem>();
        }

        private async void OnScanQRCode(object sender, EventArgs e)
        {
            // Retrieve input from the Entry field
            string scannedId = IdEntry.Text?.Trim();

            // Validate input
            if (string.IsNullOrEmpty(scannedId) || scannedId.Length != 16 || !scannedId.All(char.IsDigit))
            {
                await DisplayAlert("Invalid", "Scanned ID must be 16 numeric digits.", "OK");
                return;
            }

            // Check for duplicates in the database
            var existingItem = await _database.Table<ScannedItem>().Where(x => x.Id == scannedId).FirstOrDefaultAsync();
            if (existingItem != null)
            {
                await DisplayAlert("Duplicate", "This ID already exists in the database.", "OK");
                return;
            }

            // Add scanned ID to list and database
            _scannedIds.Add(scannedId);
            await _database.InsertAsync(new ScannedItem { Id = scannedId });
            UpdateCountLabel();

            // Clear the Entry field
            IdEntry.Text = string.Empty;

            await DisplayAlert("Success", "Scanned ID added successfully.", "OK");
        }


        private async void OnSubmit(object sender, EventArgs e)
        {
            if (_scannedIds.Count == 0)
            {
                await DisplayAlert("No Data", "There are no IDs to submit.", "OK");
                return;
            }

            await DisplayAlert("Success", "Data has been submitted to the database.", "OK");
            _scannedIds.Clear();
            UpdateCountLabel();
        }

        private async void OnReload(object sender, EventArgs e)
        {
            var items = await _database.Table<ScannedItem>().ToListAsync();
            _scannedIds.Clear();
            foreach (var item in items)
            {
                _scannedIds.Add(item.Id);
            }
            UpdateCountLabel();
        }

        private void OnClear(object sender, EventArgs e)
        {
            if (_scannedIds.Count == 0)
            {
                DisplayAlert("No Data", "The list is already empty.", "OK");
                return;
            }

            _scannedIds.Clear();
            UpdateCountLabel();
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            if (_scannedIds.Count == 0)
            {
                await DisplayAlert("Error", "No IDs to delete.", "OK");
                return;
            }

            string lastId = _scannedIds.Last();
            _scannedIds.Remove(lastId);

            // Delete the item from the database
            var itemToDelete = await _database.Table<ScannedItem>().Where(x => x.Id == lastId).FirstOrDefaultAsync();
            if (itemToDelete != null)
            {
                await _database.DeleteAsync(itemToDelete);
            }

            UpdateCountLabel();
        }

        private async void OnNext(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DetailsPage());
        }

        private void UpdateCountLabel()
        {
            CountLabel.Text = $"Count: {_scannedIds.Count}";
        }
    }

}
