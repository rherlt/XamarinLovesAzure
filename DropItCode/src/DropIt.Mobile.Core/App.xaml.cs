using DropIt.Mobile.Core.Services;
using KeyChain.Net;
using Splat;
using Xamarin.Forms;

namespace DropIt.Mobile.Core
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent ();

            //register services
            Locator.CurrentMutable.RegisterLazySingleton(() => new DeviceService(), typeof(IDeviceService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new DropService(Locator.Current.GetService<IDeviceService>()), typeof(IDropService));

            //navigate to first page
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}

