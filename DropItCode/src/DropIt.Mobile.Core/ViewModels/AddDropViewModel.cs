using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DropIt.Mobile.Core.Extensions;
using DropIt.Mobile.Core.Services;
using DropIt.Web.Client.DataContracts;
using Plugin.Geolocator.Abstractions;
using Plugin.LocalNotifications;
using PropertyChanged;
using Splat;
using Xamarin.Forms;

namespace DropIt.Mobile.Core.ViewModels
{
    [ImplementPropertyChanged]
    public class AddDropViewModel
    {
        private readonly Position _currentPosition;
        private readonly INavigation _navigation;
        private readonly IDropService _dropService;

        public AddDropViewModel(Position currentPosition, INavigation navigation)
        {
            _currentPosition = currentPosition;
            _navigation = navigation;
            _dropService = Locator.Current.GetService<IDropService>();
            ValidTo = DateTime.Now.AddDays(7);
            IsValidForever = false;
        }

        public ICommand CreateDropCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await CreateNewDrop(_currentPosition);
                });
            }
        }

        public string Title { get; set; }

        public DateTime? Date { get; set; }

        public string Message { get; set; }

        public bool IsValidForever { get; set; }

        public DateTime? ValidTo { get; set; }

        private async Task CreateNewDrop(Position position)
        {
            var drop = new Drop
            {
                Alt = position.Altitude.ToNullableNumber(),
                Date = DateTime.UtcNow,
                Lat = position.Latitude.ToNullableNumber(),
                Lon = position.Longitude.ToNullableNumber(),
                Title = Title,
                Message = Message,
                IsValidForever = IsValidForever,
                ValidTo =  ValidTo,
            };

            await _dropService.CreateDrop(drop);

            var title = "Drop successfully created!";
            var message = $"{drop.Title}\r\n{drop.Message}";

            var customNotifier = Locator.Current.GetService<ILocalNotifier>();
            if (customNotifier != null)
                customNotifier.Notify(title, message);
            else
                CrossLocalNotifications.Current.Show(title, message);

            await _navigation.PopAsync();
        }
    }
}
