using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Android.Gms.Maps.Model;
using DropIt.Mobile.Core.Map;
using DropIt.Mobile.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace DropIt.Mobile.Droid.CustomRenderer
{
	public class CustomMapRenderer : MapRenderer
	{
		private CustomMap _customMap;
        
		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// release event bindings
				if (NativeMap != null)
					NativeMap.InfoWindowClick -= OnInfoWindowClick;

                var notifyingCollection = _customMap?.CustomPins as INotifyCollectionChanged;
				if (notifyingCollection != null)
					notifyingCollection.CollectionChanged -= PinCollectionChanged;
			}

			if (e.NewElement != null)
			{
				// create event bindings
				if (NativeMap != null)
					NativeMap.InfoWindowClick += OnInfoWindowClick;

				_customMap = e.NewElement as CustomMap;
				var notifyingCollection = _customMap?.CustomPins as INotifyCollectionChanged;
				if (notifyingCollection != null)
					notifyingCollection.CollectionChanged += PinCollectionChanged;

                CreateNewMarker(_customMap.CustomPins);
			}
		}

        private void OnInfoWindowClick(object sender, Android.Gms.Maps.GoogleMap.InfoWindowClickEventArgs e)
		{
			// invoke command in customMap
			var pin = _customMap.CustomPins.FirstOrDefault(x => e.Marker.Equals((Marker)x.PlatformMarker));

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
					CreateRemoveMarker(e.OldItems.OfType<DropPin>().ToArray());
					break;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
				default:
					break;
			}
		}

		private void CreateNewMarker(IEnumerable<DropPin> pinsToAdd)
		{
			if (pinsToAdd == null)
				return;
			
			foreach (var dropPin in pinsToAdd)
			{
				var markerOptions = new MarkerOptions();
				markerOptions.SetTitle(dropPin.Pin.Label ?? " ");
				markerOptions.SetSnippet(dropPin.Pin.Address);
				markerOptions.SetPosition(new LatLng(dropPin.Pin.Position.Latitude, dropPin.Pin.Position.Longitude));
				//markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
				var marker = NativeMap.AddMarker(markerOptions);
				dropPin.PlatformMarker = marker;

				dropPin.Pin.PropertyChanged += PinPropertyChanged;
			}
		}

		private void CreateRemoveMarker(IEnumerable<DropPin> pinsToRemove)
		{
			if (pinsToRemove == null)
				return;

			foreach (var pin in pinsToRemove)
			{
				var markerToRemove = pin.PlatformMarker as Marker;
                if (markerToRemove != null)
				{
					markerToRemove.Remove();
					pin.Pin.PropertyChanged -= PinPropertyChanged;
				}
			}
		}
        
        private void PinPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var dropPin = sender as DropPin;
			if (dropPin != null && dropPin.Pin != null && dropPin.PlatformMarker != null)
			{
			    var marker = (Marker)dropPin.PlatformMarker;
				if (e.PropertyName == Pin.AddressProperty.PropertyName)
                    marker.Snippet = dropPin.Pin.Address;
				else if (e.PropertyName == nameof(Pin.Label))
					marker.Title = dropPin.Pin.Label;
			}
		}
	}
}
