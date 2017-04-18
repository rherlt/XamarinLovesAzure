namespace DropIt.Mobile.Core
{
    public interface ILocalNotifier
    {
        void Notify(string title, string message);
    }
}