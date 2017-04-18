using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace DropIt.Mobile.Core.Map
{
	public class CustomMap : Xamarin.Forms.Maps.Map
	{
		
        public static readonly BindableProperty InfoWindowClickedProperty = BindableProperty.Create("InfoWindowClicked", typeof(ICommand), typeof(CustomMap), null);

        public ICommand InfoWindowClicked
        {
            get { return (ICommand)GetValue(InfoWindowClickedProperty); }
            set { SetValue(InfoWindowClickedProperty, value); }
        }

        public static readonly BindableProperty CustomPinsProperty = BindableProperty.Create("CustomPins", typeof(ObservableCollection<DropPin>), typeof(CustomMap), null);

        public ObservableCollection<DropPin> CustomPins
		{
            get { return (ObservableCollection<DropPin>)GetValue(CustomPinsProperty); }
            set { SetValue(CustomPinsProperty, value); }
        }

		public CustomMap()
		{
			CustomPins = new ObservableCollection<DropPin>();
		}
	}
}
