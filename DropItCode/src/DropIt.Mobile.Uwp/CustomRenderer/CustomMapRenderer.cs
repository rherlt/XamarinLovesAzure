using DropIt.Mobile.Core;
using DropIt.Mobile.Uwp.CustomRenderer;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using DropIt.Mobile.Core.Map;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace DropIt.Mobile.Uwp.CustomRenderer
{
    public class CustomMapRenderer : MapRenderer
    {
        private CustomMap _customMap;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // release event bindings
                if (Control != null)
                    Control.MapElementClick -= OnMapElementClick;

                var notifyingCollection = _customMap?.CustomPins as INotifyCollectionChanged;
                if (notifyingCollection != null)
                    notifyingCollection.CollectionChanged += PinCollectionChanged;
            }

            if (e.NewElement != null)
            {
                // create event bindings
                if (Control != null)
                    Control.MapElementClick += OnMapElementClick;

                _customMap = e.NewElement as CustomMap;

                var notifyingCollection = _customMap?.CustomPins as INotifyCollectionChanged;
                if (notifyingCollection != null)
                    notifyingCollection.CollectionChanged += PinCollectionChanged;

                CreateNewMarker(_customMap?.CustomPins);
            }
        }
        
        private void OnMapElementClick (MapControl control, MapElementClickEventArgs args)
        {
            var clickedIcon = args.MapElements.FirstOrDefault() as MapIcon;
            if (clickedIcon == null)
                return;

            var pin = _customMap.CustomPins.FirstOrDefault(x => x.PlatformMarker == clickedIcon);
            if (pin != null)
                _customMap?.InfoWindowClicked?.Execute(pin);
        }

        private void PinCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CreateNewMarker(e.NewItems.OfType<DropPin>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveMarker(e.OldItems.OfType<DropPin>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void UpdatePinTitle(DropPin dropPin)
        {
            var title = dropPin.Pin.Label ?? " ";
            var mapIcon = dropPin.PlatformMarker as MapIcon;
            if (mapIcon != null)
                mapIcon.Title = string.IsNullOrWhiteSpace(dropPin.Pin.Address) ? title : $"{title}\n{dropPin.Pin.Address}";
        }

        private void CreateNewMarker(IEnumerable<DropPin> pinsToAdd)
        {
            if (pinsToAdd == null)
                return;

            foreach (var dropPin in pinsToAdd)
            {
                var geoPosition = new BasicGeoposition() { Latitude = dropPin.Pin.Position.Latitude, Longitude = dropPin.Pin.Position.Longitude };
                var mapIcon = new MapIcon {Location = new Geopoint(geoPosition)};

                dropPin.PlatformMarker = mapIcon;
                UpdatePinTitle(dropPin);                
                Control.MapElements.Add(mapIcon);
                dropPin.Pin.PropertyChanged += PinPropertyChanged;
            }
        }

        private void RemoveMarker(IEnumerable<DropPin> pinsToRemove)
        {
            if (pinsToRemove == null)
                return;

            foreach (var dropPin in pinsToRemove)
            {
                var iconToRemove = dropPin.PlatformMarker as MapIcon;
                if (iconToRemove != null)
                {
                    Control.MapElements.Remove(iconToRemove);
                    dropPin.Pin.PropertyChanged -= PinPropertyChanged;
                }
            }
        }

        private void PinPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var pin = sender as DropPin;
            if (pin != null && pin.Pin != null && pin.PlatformMarker != null)
            {
                if (e.PropertyName == Pin.AddressProperty.PropertyName || e.PropertyName == nameof(Pin.Label))
                    UpdatePinTitle(pin);
            }
        }
    }
}
