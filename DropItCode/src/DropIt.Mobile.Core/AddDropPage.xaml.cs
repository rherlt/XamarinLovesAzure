using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using DropIt.Mobile.Core.Extensions;
using DropIt.Mobile.Core.Services;
using DropIt.Mobile.Core.ViewModels;
using Xamarin.Forms;
using DropIt.Web.Client.DataContracts;
using Plugin.LocalNotifications;
using DropIt.Web.Client.Net;
using DropIt.Web.Client.Extensions;
using Plugin.Geolocator.Abstractions;
using Splat;

namespace DropIt.Mobile.Core
{
	public partial class AddDropPage 
	{
        public AddDropPage(Position currentPosition)
		{
            InitializeComponent();
            ViewModel = new AddDropViewModel(currentPosition, Navigation);
		    BindingContext = ViewModel;
		}

	    public AddDropViewModel ViewModel { get; set; } 
        
	}
}
