using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DropIt.Mobile.Core.Map;
using DropIt.Mobile.Core.Services;
using DropIt.Web.Client.DataContracts;
using DropIt.Web.Client.Net;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Splat;
using Xamarin.Forms;
using PropertyChanged;
using Xamarin.Forms.Maps;
using System.Collections.Specialized;
using System.Collections;

namespace DropIt.Mobile.Core.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        #region Readonly and Events

        private readonly INavigation _navigation;
        private readonly IDropService _dropService;

        public event EventHandler<PositionEventArgs> UserPositionChanged;

        #endregion

        #region Private Fields

        private double _mapNorth;
        private double _mapSouth;
        private double _mapWest;
        private double _mapEast;
        private bool _hasMovedToUserPositionAtStart;
        private bool _locationRightsChecked;
        private IGeolocator _locator;

        #endregion

        #region Ctor

        public MainViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _dropService = Locator.Current.GetService<IDropService>();
            DropPins = new ObservableCollection<DropPin>();
            Drops.CollectionChanged += OnServiceDropsCollectionChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<Drop> Drops => _dropService.Drops;
        public ObservableCollection<DropPin> DropPins { get; set; }
        public bool IsShowingUserLocation { get; set; }
        public bool LocationPermissionGranted { get; private set; }

        #endregion

        #region Commands

        public ICommand InfoWindowClickedCommand
        {
            get
            {
                return new Command(pinObj =>
                {
                    var dropPin = pinObj as DropPin;
                    if (!string.IsNullOrWhiteSpace(dropPin?.Pin?.Address))
                    {
                        Debug.WriteLine("clicked on: " + dropPin.Pin.Label);
                        _navigation.PushAsync(new DisplayDropPage(dropPin.Drop));
                    }
                });
            }
        }

        public ICommand AddDropCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var currentPosition = await _locator.GetPositionAsync();
                    await _navigation.PushAsync(new AddDropPage(currentPosition));
                }, () => LocationPermissionGranted);
            }
        }

        public ICommand ShowOwnDropsCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await _navigation.PushAsync(new DropListPage());
                });
            }
        }

        #endregion

        #region Permissions

        public async Task AskForLocationPermissionIfNeeded()
        {
            try
            {
                if (!_locationRightsChecked)
                {
                    _locationRightsChecked = true;

                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                    if (status != PermissionStatus.Granted)
                    {
                        var results =
                            await CrossPermissions.Current.RequestPermissionsAsync(new[] {Permission.Location});
                        status = results[Permission.Location];
                    }

                    if (status == PermissionStatus.Granted)
                    {
                        LocationPermissionGranted = true;
                        IsShowingUserLocation = true;

                        _locator = CrossGeolocator.Current;
                        _locator.DesiredAccuracy = 50;
                        _locator.AllowsBackgroundUpdates = true;
                        _locator.PositionChanged += HandlePositionChanged;
                        await _locator.StartListeningAsync(10, 10);
                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                        LocationPermissionGranted = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region Geo Calculations

        // source: https://forums.xamarin.com/discussion/22493/maps-visibleregion-bounds
        private void SetMapCoordinates(MapSpan region)
        {
            var center = region.Center;
            var halfheightDegrees = region.LatitudeDegrees / 2;
            var halfwidthDegrees = region.LongitudeDegrees / 2;

            _mapWest = center.Longitude - halfwidthDegrees;
            _mapEast = center.Longitude + halfwidthDegrees;
            _mapNorth = center.Latitude + halfheightDegrees;
            _mapSouth = center.Latitude - halfheightDegrees;

            // Adjust for Internation Date Line (+/- 180 degrees longitude)
            if (_mapWest < -180) _mapWest = 180 + (180 + _mapWest);
            if (_mapEast > 180) _mapEast = (_mapEast - 180) - 180;

            Debug.WriteLine("Bounding box:");
            Debug.WriteLine("                    " + _mapNorth);
            Debug.WriteLine("  " + _mapWest + "                " + _mapEast);
            Debug.WriteLine("                    " + _mapSouth);
        }


        private double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double Rad2Deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        // math logic to calculate distance between two pins
        // source: http://www.geodatasource.com/developers/c-sharp
        private double GetDistance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) +
                          Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        #endregion

        // show message for the drops near to the user position
        //private async Task RefreshPins()
        //{
        //    var pinsToRemove = new List<Pin>();
        //    var pinsToAdd = new List<Pin>();
        //    var currentPosition = await _locator.GetPositionAsync();
        //    var myPosition = new Xamarin.Forms.Maps.Position(currentPosition.Latitude, currentPosition.Longitude);

        //    foreach (var currentPin in DropPins)
        //    {
        //        var dist = GetDistance(myPosition.Latitude, myPosition.Longitude, currentPin.Pin.Position.Latitude,
        //            currentPin.Pin.Position.Longitude, 'K');
        //        if (dist < Const.Distance)
        //        {
        //            Debug.WriteLine("close enough: " + currentPin.Pin.Label);
        //            if (string.IsNullOrEmpty(currentPin.Pin.Address))
        //            {
        //                //remove pin and add pin again with address
        //                ReAddPinToMap(currentPin, pinsToRemove, pinsToAdd, true);
        //            }
        //        }
        //        else
        //        {
        //            Debug.WriteLine("far away");
        //            if (!string.IsNullOrEmpty(currentPin.Address))
        //            {
        //                ReAddPinToMap(currentPin, pinsToRemove, pinsToAdd, false);
        //            }
        //        }
        //    }

        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        foreach (var pinToRemove in pinsToRemove)
        //        {
        //            RemovePin(pinToRemove);
        //        }

        //        foreach (var pinToAdd in pinsToAdd)
        //        {
        //            AddPin(pinToAdd);
        //        }
        //    });
        //}


        //void ReAddPinToMap(Pin currentPin, List<Pin> pinsToRemove, List<Pin> pinsToAdd, bool showAddress)
        //{
        //    pinsToRemove.Add(currentPin);
        //    Drop pinDrop = null;
        //    if (_dropDictionary.TryGetValue(currentPin, out pinDrop))
        //    {
        //        _dropDictionary.Remove(currentPin);
        //        var newPin = new Pin
        //        {
        //            Label = pinDrop.Title,
        //            Address = showAddress ? pinDrop.Message : string.Empty,
        //            Position = new Xamarin.Forms.Maps.Position(pinDrop.Lat.Value, pinDrop.Lon.Value)

        //        };

        //        if (showAddress)
        //            newPin.Clicked += (object sender, EventArgs e) =>
        //            {
        //                Debug.WriteLine("clicked on: " + newPin.Label);
        //                _navigation.PushAsync(new DisplayDropPage(pinDrop));
        //            };

        //        pinsToAdd.Add(newPin);
        //    }
        //}

        #region Drops and Collections

        // add all drops from server to the map
        public async Task ShowAllDrops(MapSpan currentMapVisibleRegion)
        {
            if (currentMapVisibleRegion != null)
            {
                SetMapCoordinates(currentMapVisibleRegion);
                var dropsResult = await _dropService.GetDropsForMap(_mapNorth, _mapSouth, _mapWest, _mapEast);
                foreach (var drop in dropsResult.Drops)
                {
                    if (DropPins.All(x => x.Drop.Id != drop.Id
                        && (x.Drop.Lat != drop.Lat // needed
                        && x.Drop.Lon != drop.Lon // due to
                        && x.Drop.Title != drop.Title))) // xamarin features
                    {
                        DropPins.Add(CreateDropPin(drop));
                    }
                }
            }
        }

        private DropPin CreateDropPin(Drop drop)
        {
            var pin = new Pin
            {
                Position = new Xamarin.Forms.Maps.Position(drop.Lat.Value, drop.Lon.Value),
                Label = drop.Title,
                Address = drop.Message
            };

            return new DropPin(drop, pin);
        }

        private void RemoveDrops(IList dropsToRemove)
        {
            foreach (Drop removedDrop in dropsToRemove)
            {
                var removedDropPin = DropPins.FirstOrDefault(x => x.Drop.Id == removedDrop.Id);

                if (removedDropPin != null)
                    DropPins.Remove(removedDropPin);
            }
        }

        private void AddDrops(IList dropToAdd)
        {
            foreach (Drop drop in dropToAdd)
                if (DropPins.All(x => x.Drop.Id != drop.Id))
                   DropPins.Add(CreateDropPin(drop));
        }

        #endregion

        #region Event Handler

        private void HandlePositionChanged(object sender, PositionEventArgs e)
        {
            if (IsShowingUserLocation && UserPositionChanged != null && !_hasMovedToUserPositionAtStart)
            {
                OnUserPositionChanged(e);
                _hasMovedToUserPositionAtStart = true;
            }
        }

        private void OnServiceDropsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        AddDrops(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        RemoveDrops(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                        RemoveDrops(e.OldItems);
                    if (e.NewItems != null)
                        AddDrops(e.NewItems);
                    break;
            }
        }

        protected virtual void OnUserPositionChanged(PositionEventArgs e)
        {
            UserPositionChanged?.Invoke(this, e);
        }

        #endregion
    }
}
