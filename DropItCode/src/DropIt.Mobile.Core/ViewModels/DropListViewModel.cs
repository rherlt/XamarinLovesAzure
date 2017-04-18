using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DropIt.Mobile.Core.Services;
using DropIt.Web.Client.DataContracts;
using PropertyChanged;
using Splat;
using Xamarin.Forms;

namespace DropIt.Mobile.Core.ViewModels
{
    [ImplementPropertyChanged]
    public class DropListViewModel
    {
        private readonly IDropService _dropService;

        public DropListViewModel()
        {
            _dropService = Locator.Current.GetService<IDropService>();
        }

        public ObservableCollection<Drop> Drops => _dropService.OwnDrops;

        public ICommand DeleteDropCommand {
            get
            {
                return new Command<Drop>(async drop =>
                {
                    await _dropService.DeleteDrop(drop.Id);
                });
            }
        }
    }
}
