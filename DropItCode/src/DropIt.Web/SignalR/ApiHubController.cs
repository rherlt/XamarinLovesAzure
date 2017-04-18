using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR.Infrastructure;

namespace DropIt.Web.SignalR
{
    public abstract class ApiHubController<T> : Controller where T : Hub
    {
        protected readonly IHubContext Hub;
        public IHubConnectionContext<dynamic> Clients { get; private set; }

        public IGroupManager Groups { get; private set; }

        protected ApiHubController(IConnectionManager signalRConnectionManager)
        {
            Hub = signalRConnectionManager.GetHubContext<T>();
            Clients = Hub.Clients;
            Groups = Hub.Groups;
        }
    }
}