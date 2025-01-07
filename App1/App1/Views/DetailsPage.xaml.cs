using System;
using Xamarin.Forms;

namespace App1.Views
{
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            DetailsListView.ItemsSource = await App.Database.GetIdsAsync();
        }
    }
}
