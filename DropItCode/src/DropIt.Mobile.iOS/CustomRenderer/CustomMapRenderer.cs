using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CoreLocation;
using DropIt.Mobile.Core.Map;
using DropIt.Mobile.iOS.CustomRenderer;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace DropIt.Mobile.iOS.CustomRenderer
{
    public class CustomMapRenderer : MapRenderer
    {
        private const string DropViewReuseIdentifier = "dropPinReuseId";

        private CustomMap _customMap;
        private MKMapView _nativeMap;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // release event bindings
                if (_nativeMap != null)
                {
                    _nativeMap.GetViewForAnnotation = null;
                    _nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                    _nativeMap = null;
                }

                var notifyingCollection = _customMap?.CustomPins as INotifyCollectionChanged;
                if (notifyingCollection != null)
                    notifyingCollection.CollectionChanged += PinCollectionChanged;
            }

            if (e.NewElement != null)
            {
                // create event bindings
                _nativeMap = Control as MKMapView;
                _customMap = e.NewElement as CustomMap;

                if (_nativeMap != null)
                {
                    _nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                    _nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                }

                var notifyingCollection = _customMap?.CustomPins as INotifyCollectionChanged;
                if (notifyingCollection != null)
                    notifyingCollection.CollectionChanged += PinCollectionChanged;

                CreateNewAnnotations(_customMap?.CustomPins);
            }
        }

        public MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            var isUserLocation = (annotation as MKPointAnnotation) == null;
            if (isUserLocation)
                return null;

            // create pin annotation view
            var pinView = mapView.DequeueReusableAnnotation(DropViewReuseIdentifier) as MKPinAnnotationView;

            if (pinView == null)
                pinView = new MKPinAnnotationView(annotation, DropViewReuseIdentifier);

            pinView.PinColor = MKPinAnnotationColor.Red;
            pinView.CanShowCallout = true;
            pinView.RightCalloutAccessoryView = new UIButton(UIButtonType.DetailDisclosure);

            return pinView;
        }

        #region Annotation Managment

        private void CreateNewAnnotations(IEnumerable<DropPin> pinsToAdd)
        {
            if (pinsToAdd == null)
                return;

            foreach (var dropPin in pinsToAdd)
            {
                var pin = dropPin.Pin;
                var geoPosition = new CLLocationCoordinate2D() { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };
                var mapAnnotation = new MKPointAnnotation { Coordinate = geoPosition, Title = pin.Label, Subtitle = pin.Address };

                dropPin.PlatformMarker = mapAnnotation;
                _nativeMap?.AddAnnotation(mapAnnotation);

                pin.PropertyChanged += PinPropertyChanged;
            }
        }

        private void RemoveAnnotations(IEnumerable<DropPin> pinsToRemove)
        {
            if (pinsToRemove == null)
                return;

            foreach (var dropPin in pinsToRemove)
            {
                var annotationToRemove = dropPin.PlatformMarker as MKPointAnnotation;
                if (annotationToRemove != null)
                {
                    _nativeMap?.RemoveAnnotation(annotationToRemove);
                    dropPin.Pin.PropertyChanged -= PinPropertyChanged;
                }
            }
        }

        #endregion

        #region Event Handler 

        private void PinCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CreateNewAnnotations(e.NewItems.OfType<DropPin>().ToArray());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveAnnotations(e.OldItems.OfType<DropPin>().ToArray());
                    break;
            }
        }

        private void PinPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var dropPin = sender as DropPin;
            var annotation = dropPin.PlatformMarker as MKPointAnnotation;

            if (dropPin != null && dropPin.Pin != null && annotation != null)
            {
                if (e.PropertyName == Pin.AddressProperty.PropertyName)
                    annotation.Subtitle = dropPin.Pin.Address;
                else if (e.PropertyName == nameof(Pin.Label))
                    annotation.Title = dropPin.Pin.Label;
            }
        }

        private void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var annotation = e.View?.Annotation;

            if (annotation == null)
                return;

            var pin = _customMap.CustomPins.FirstOrDefault(x => x.PlatformMarker == annotation);
            if (pin != null)
                _customMap?.InfoWindowClicked?.Execute(pin);
        }

        #endregion
    }
}
