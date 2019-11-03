using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PortalToWork.Models;
using PortalToWork.ViewModels;
using Xamarin.Forms.GoogleMaps;
using System.Reflection;
using Xamarin.Essentials;

namespace PortalToWork.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(viewModel.Job.SiteURL));
        }

        private void Fav_Tapped(object sender, EventArgs e)
        {
            Favorite.IsVisible = false;
            int numFavorites = Preferences.Get("favorites", 0);
            numFavorites++;
            string jobID = viewModel.Job.JobID;
            Preferences.Set("favorite" + numFavorites, jobID);
            Preferences.Set("favorites", numFavorites);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var poppedPage = await Navigation.PopAsync();
        }
    }
}