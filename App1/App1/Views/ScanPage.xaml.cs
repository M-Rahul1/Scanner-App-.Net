using System;
using Xamarin.Forms;

namespace App1.Views
{
    public partial class ScanPage : ContentPage
    {
        public ScanPage()
        {
            InitializeComponent();
        }

        // Add a sample event to test functionality
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DisplayAlert("Welcome", "ScanPage is loaded successfully!", "OK");
        }
    }
}
