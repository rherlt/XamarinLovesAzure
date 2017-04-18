using DropIt.Web.Client.DataContracts;
using Microsoft.AspNetCore.SignalR;

namespace DropIt.Web.SignalR
{
    public class DropHub : Hub<IDropHub>
    {
    }
}
