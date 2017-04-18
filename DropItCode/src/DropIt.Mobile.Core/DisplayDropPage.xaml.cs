using System;
using System.Collections.Generic;
using DropIt.Mobile.Core.ViewModels;
using DropIt.Web.Client.DataContracts;
using Xamarin.Forms;

namespace DropIt.Mobile.Core
{
    public partial class DisplayDropPage : ContentPage
    {
        Drop drop;

        public DisplayDropPage(Drop drop)
        {
            InitializeComponent();
            ViewModel = new DisplayDropPageViewModel(drop);
            BindingContext = ViewModel;
        }

        public DisplayDropPageViewModel ViewModel { get; set; }
    }
}
