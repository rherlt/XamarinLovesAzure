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
    public class DisplayDropPageViewModel
    {
        public DisplayDropPageViewModel(Drop drop)
        {
            Title = drop.Title;
            Date = drop.Date;
            Message = drop.Message;
        }

        public string Title { get; set; }

        public DateTime ?Date { get; set; }

        public string Message { get; set; }
    }
}
