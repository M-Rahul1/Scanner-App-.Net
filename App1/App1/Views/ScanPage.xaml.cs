using System;
using System.Collections.ObjectModel;
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

        // Simulate QR Code Scanning
        private void OnScanQRCode(object sender, EventArgs e)
        {
            var scannedId = $"ID-{DateTime.Now.Ticks % 10000000000000000:D16}"; // Generate a fake 16-digit ID
            if (!_scannedIds.Contains(scannedId))
            {
                _scannedIds.Add(scannedId);
                UpdateCountLabel();
            }
            else
            {
                DisplayAlert("Duplicate", "This ID is already scanned.", "OK");
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

        // Submit IDs (Placeholder for Future Logic)
        private void OnSubmit(object sender, EventArgs e)
        {
            DisplayAlert("Submit", "IDs submitted successfully!", "OK");
            _scannedIds.Clear();
            UpdateCountLabel();
        }

        // Reload IDs (Placeholder for Database Fetch)
        private void OnReload(object sender, EventArgs e)
        {
            _scannedIds.Clear();
            _scannedIds.Add("1234567890123456");
            _scannedIds.Add("6543210987654321");
            UpdateCountLabel();
        }

        // Navigate to the Next Page
        private void OnNext(object sender, EventArgs e)
        {
            DisplayAlert("Next", "Navigating to the next page...", "OK");
        }

        // Update Count Label
        private void UpdateCountLabel()
        {
            CountLabel.Text = $"Count: {_scannedIds.Count}";
        }
    }
}
