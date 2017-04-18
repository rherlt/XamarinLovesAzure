using System;
using System.Collections.Generic;
using DropIt.Mobile.Core.ViewModels;
using DropIt.Web.Client.DataContracts;
using DropIt.Web.Client.Net;
using Xamarin.Forms;

namespace DropIt.Mobile.Core
{
	public partial class DropListPage : ContentPage
	{
		public DropListPage()
		{
            InitializeComponent();

            ViewModel = new DropListViewModel();
            BindingContext = ViewModel;
		}
        
	    public DropListViewModel ViewModel { get; set; }

		public async void OnDelete(object sender, EventArgs e)
		{
			var mi = sender as MenuItem;
		    if (mi != null)
		    {
                var shouldDelete = await DisplayAlert("Delete a drop", "Are you sure you want to delete this drop?", "OK", "Cancel");
		        var drop = mi.BindingContext as Drop;
                if (shouldDelete && drop != null)
                    ViewModel.DeleteDropCommand.Execute(drop);
		    }
		}
	}
}