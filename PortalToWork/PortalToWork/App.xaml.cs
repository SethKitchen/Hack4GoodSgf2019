using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PortalToWork.Services;
using PortalToWork.Views;

namespace PortalToWork
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<JobsDataStore>();
            GoogleMapsApiService.Initialize(Constants.GoogleMapsApiKey);
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
