using System;
using App1.Services;
using App1.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new TabbedPage
            {
                Children =
                {
                    new NavigationPage(new ScanPage()) { Title = "Scan" },
                    new NavigationPage(new DetailsPage()) { Title = "Details" }
                }
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
