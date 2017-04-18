using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DropIt.Web.Client.DataContracts;

namespace DropIt.Mobile.Core.Services
{
    public interface IDropService
    {
        ObservableCollection<Drop> Drops { get; }
        ObservableCollection<Drop> OwnDrops { get; }
        Task<bool> DeleteDrop(Guid? dropId);
        Task<Drop> GetDrop(Guid? id);
        Task CreateDrop(Drop drop);
        Task<GetDropsResponse> GetDropsForMap(double mapNorth, double mapSouth, double mapWest, double mapEast);


    }
}