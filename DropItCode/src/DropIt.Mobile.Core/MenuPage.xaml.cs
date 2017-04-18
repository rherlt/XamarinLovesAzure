//using System;
//using System.Collections.Generic;

//using Xamarin.Forms;

//namespace Dowser.HereIAm.Mobile
//{
//	public partial class MenuPage : MasterDetailPage
//	{
//		public MenuPage()
//		{
//			InitializeComponent();
//			MobileIdEntry.Text = AppState.MobileId;
//		}

//		void MobileIdTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
//		{
//			AppState.MobileId = e.NewTextValue;
//		}

//		void ShowNotificationsSwitchToggled(object sender, Xamarin.Forms.ToggledEventArgs e)
//		{
//			AppState.ShowNotificationsSwitchEnabled = e.Value;
//		}

//		void UpdatePositionSwitchToggled(object sender, Xamarin.Forms.ToggledEventArgs e)
//		{
//			AppState.UpdatePositionSwitchEnabled = e.Value;
//		}
//	}
//}
