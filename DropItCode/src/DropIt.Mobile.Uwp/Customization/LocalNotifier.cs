using System;
using Windows.UI.Popups;
using DropIt.Mobile.Core;

namespace DropIt.Mobile.Uwp.Customization

{
    public class LocalNotifier : ILocalNotifier
    {
        public void Notify(string title, string message)
        {
           new MessageDialog(message, title).ShowAsync()
                .AsTask()
                .ConfigureAwait(false);
        }
    }
}
