using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DropIt.Mobile.Core.Extensions;
using DropIt.Mobile.Core.ViewModels;
using DropIt.Web.Client.DataContracts;
using DropIt.Web.Client.Extensions;
using DropIt.Web.Client.Net;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.LocalNotifications;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;


namespace DropIt.Mobile.Core
{
    public partial class MainPage : ContentPage
    {
       public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            ViewModel = new MainViewModel(Navigation);
            BindingContext = ViewModel;

            CurrentMap.PropertyChanged += VisibleRegion_Changed;
            ViewModel.UserPositionChanged += ViewModelOnUserPositionChanged;
        }

        public MainViewModel ViewModel { get; set; }

        private void ViewModelOnUserPositionChanged(object sender, PositionEventArgs e)
        {
            var pos = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);
            CurrentMap.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromKilometers(1)));
        }

        private void VisibleRegion_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentMap.VisibleRegion))
                ViewModel.ShowAllDrops(CurrentMap.VisibleRegion).ConfigureAwait(false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.AskForLocationPermissionIfNeeded().ConfigureAwait(false);
            ViewModel.ShowAllDrops(CurrentMap.VisibleRegion).ConfigureAwait(false);
        }
    }
}