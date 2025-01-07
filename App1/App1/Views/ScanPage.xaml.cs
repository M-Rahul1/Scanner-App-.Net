using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using App1.Models;

namespace App1.Views
{
    public partial class ScanPage : ContentPage
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly ObservableCollection<string> _scannedIds;

        public ScanPage()
        {
            InitializeComponent();
            _database = InitializeDatabase();
            _scannedIds = new ObservableCollection<string>();
            IdListView.ItemsSource = _scannedIds;
            UpdateCountLabel();
        }

        private SQLiteAsyncConnection InitializeDatabase()
        {
            try
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScannedIDs.db3");
                var database = new SQLiteAsyncConnection(dbPath);
                database.CreateTableAsync<ScannedItem>().Wait();
                return database;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Database initialization failed: {ex.Message}", "OK");
                return null;
            }
        }

        private async void OnScanQRCode(object sender, EventArgs e)
        {
            string scannedId = IdEntry.Text?.Trim();

            if (!IsValidScannedId(scannedId)) return;

            if (await IsDuplicateScannedIdAsync(scannedId))
            {
                await DisplayAlert("Duplicate", "This ID already exists in the database.", "OK");
                return;
            }

            if (await AddScannedIdToDatabaseAsync(scannedId))
            {
                _scannedIds.Add(scannedId);
                UpdateCountLabel();
                IdEntry.Text = string.Empty;
                await DisplayAlert("Success", "Scanned ID added successfully.", "OK");
            }
        }

        private async void OnSubmit(object sender, EventArgs e)
        {
            if (!_scannedIds.Any())
            {
                await DisplayAlert("No Data", "There are no IDs to submit.", "OK");
                return;
            }

            await DisplayAlert("Success", "Data has been successfully submitted.", "OK");
            _scannedIds.Clear();
            UpdateCountLabel();
        }

        private async void OnReload(object sender, EventArgs e)
        {
            try
            {
                var items = await _database.Table<ScannedItem>().ToListAsync();
                _scannedIds.Clear();
                foreach (var item in items)
                {
                    _scannedIds.Add(item.Id);
                }
                UpdateCountLabel();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to reload data: {ex.Message}", "OK");
            }
        }

        private void OnClear(object sender, EventArgs e)
        {
            if (!_scannedIds.Any())
            {
                DisplayAlert("No Data", "The list is already empty.", "OK");
                return;
            }

            _scannedIds.Clear();
            UpdateCountLabel();
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            if (!_scannedIds.Any())
            {
                await DisplayAlert("Error", "No IDs to delete.", "OK");
                return;
            }

            string lastId = _scannedIds.Last();
            _scannedIds.Remove(lastId);

            if (await DeleteScannedIdFromDatabaseAsync(lastId))
            {
                UpdateCountLabel();
            }
        }

        private async void OnNext(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DetailsPage());
        }

        private void UpdateCountLabel()
        {
            CountLabel.Text = $"Count: {_scannedIds.Count}";
        }

        // Validation for scanned IDs
        private bool IsValidScannedId(string scannedId)
        {
            if (string.IsNullOrEmpty(scannedId))
            {
                DisplayAlert("Invalid", "Scanned ID cannot be empty.", "OK");
                return false;
            }

            if (!Regex.IsMatch(scannedId, @"^\d{16}$"))
            {
                DisplayAlert("Invalid", "Scanned ID must be exactly 16 numeric digits.", "OK");
                return false;
            }

            if (_scannedIds.Contains(scannedId))
            {
                DisplayAlert("Duplicate", "This ID already exists in the list.", "OK");
                return false;
            }

            return true;
        }

        // Check for duplicate in the database
        private async Task<bool> IsDuplicateScannedIdAsync(string scannedId)
        {
            try
            {
                var existingItem = await _database.Table<ScannedItem>().Where(x => x.Id == scannedId).FirstOrDefaultAsync();
                return existingItem != null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to check for duplicates: {ex.Message}", "OK");
                return true;
            }
        }

        // Add scanned ID to the database
        private async Task<bool> AddScannedIdToDatabaseAsync(string scannedId)
        {
            try
            {
                await _database.InsertAsync(new ScannedItem { Id = scannedId });
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add ID to the database: {ex.Message}", "OK");
                return false;
            }
        }

        // Delete scanned ID from the database
        private async Task<bool> DeleteScannedIdFromDatabaseAsync(string scannedId)
        {
            try
            {
                var itemToDelete = await _database.Table<ScannedItem>().Where(x => x.Id == scannedId).FirstOrDefaultAsync();
                if (itemToDelete != null)
                {
                    await _database.DeleteAsync(itemToDelete);
                }
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete ID from the database: {ex.Message}", "OK");
                return false;
            }
        }
    }
}
