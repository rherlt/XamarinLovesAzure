using System.Threading.Tasks;

namespace DropIt.Web.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
