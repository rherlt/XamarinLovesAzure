using System;
using DropIt.Mobile.Core;
using Foundation;
using KeyChain.Net;
using KeyChain.Net.XamarinIOS;
using Splat;
using UIKit;

namespace DropIt.Mobile.iOS
{
    [Register ("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
   {
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
			Xamarin.Forms.Forms.Init ();
            Xamarin.FormsMaps.Init();
            
            var settings = UIUserNotificationSettings.GetSettingsForTypes (
                UIUserNotificationType.Alert
                | UIUserNotificationType.Badge
                | UIUserNotificationType.Sound,
                new NSSet ());
            UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);

            
            Locator.CurrentMutable.Register(() => new KeyChainHelper("DropIt by Rico Herlt", false, Security.SecAccessible.Always), typeof(IKeyChainHelper));

            LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
        }

        public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            base.DidReceiveRemoteNotification (application, userInfo, completionHandler);
        }
    }
}