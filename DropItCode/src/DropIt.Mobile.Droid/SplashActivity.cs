using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using KeyChain.Net;
using KeyChain.Net.XamarinAndroid;
using Splat;
using DropIt.Mobile.Core;

namespace DropIt.Mobile.Droid
{
	[Activity(Label = "DropIt", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : Activity
	{

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            Locator.CurrentMutable.Register(() => new KeyChainHelper(() => BaseContext, "kljhgvcfgvhjkl", "koijghvbnjkghfcvb", "DropIt"), typeof(IKeyChainHelper));
		}


		protected override void OnResume()
		{
			base.OnResume();

			Task startupWork = new Task(() => { });

			startupWork.ContinueWith(t =>
			{
				StartActivity(new Intent(Application.Context, typeof(MainActivity)));
			}, TaskScheduler.FromCurrentSynchronizationContext());

			startupWork.Start();
		}
	}
}