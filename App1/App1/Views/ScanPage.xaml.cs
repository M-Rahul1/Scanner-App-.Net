using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace App1.Views
{
    public partial class ScanPage : ContentPage
    {
        private ObservableCollection<string> _scannedIds = new ObservableCollection<string>();

        public ScanPage()
        {
            InitializeComponent();
            IdListView.ItemsSource = _scannedIds;
            UpdateCountLabel();
        }

        // Handle QR Code Scanning
        private void OnScanQRCode(object sender, EventArgs e)
        {
            var scannedId = $"1234567890123456"; // Replace with actual QR code scanning logic
            AddIdToList(scannedId);
        }

        // Add ID to List
        private void AddIdToList(string id)
        {
            if (_scannedIds.Contains(id))
            {
                DisplayAlert("Duplicate", "This ID is already scanned.", "OK");
                return;
            }

            if (id.Length == 16 && long.TryParse(id, out _))
            {
                _scannedIds.Add(id);
                UpdateCountLabel();
            }
            else
            {
                DisplayAlert("Invalid", "Please enter a valid 16-digit ID.", "OK");
            }
        }

        // Clear All IDs
        private void OnClear(object sender, EventArgs e)
        {
            _scannedIds.Clear();
            UpdateCountLabel();
        }

        // Delete Selected ID
        private void OnDelete(object sender, EventArgs e)
        {
            if (IdListView.SelectedItem != null)
            {
                _scannedIds.Remove(IdListView.SelectedItem.ToString());
                UpdateCountLabel();
            }
            else
            {
                DisplayAlert("Error", "Please select an ID to delete.", "OK");
            }
        }

        // Submit IDs to Database
        private async void OnSubmit(object sender, EventArgs e)
        {
            foreach (var id in _scannedIds)
            {
                await App.Database.SaveIdAsync(new ScannedId { Id = id });
            }

            DisplayAlert("Submitted", "Data saved successfully.", "OK");
            _scannedIds.Clear();
            UpdateCountLabel();
        }

        // Reload IDs from Database
        private async void OnReload(object sender, EventArgs e)
        {
            _scannedIds.Clear();
            var idsFromDb = await App.Database.GetIdsAsync();
            foreach (var id in idsFromDb)
            {
                _scannedIds.Add(id.Id);
            }
            UpdateCountLabel();
        }

        // Navigate to Details Page
        private async void OnNext(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//DetailsPage");
        }

        // Update Count Label
        private void UpdateCountLabel()
        {
            CountLabel.Text = $"Count: {_scannedIds.Count}";
        }
    }
}
