using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropIt.Web.Client.DataContracts;
using DropIt.Web.Client.Net;
using KeyChain.Net;

namespace DropIt.Mobile.Core.Services
{
    public class DropService : IDropService
    {
        private readonly IDeviceService _deviceService;

        public DropService(IDeviceService deviceService)
        {
            _deviceService = deviceService;
            _dropClient = new DropClient(Const.ApiUrl);
        }

        private ObservableCollection<Drop> _ownDrops;
        private readonly DropClient _dropClient;
        public ObservableCollection<Drop> Drops { get; protected set; } = new ObservableCollection<Drop>();
        public ObservableCollection<Drop> OwnDrops {
            get
            {
                if (_ownDrops == null)
                {
                    _ownDrops = new ObservableCollection<Drop>();
                    GetOwnDrops().ConfigureAwait(false);
                }
                return _ownDrops;
            }
        }

        private async Task GetOwnDrops()
        {
            var result = await _dropClient.GetOwnDrops(_deviceService.DeviceId);

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var drop in result.Drops)
                    if (OwnDrops.All(x => x.Id != drop.Id))
                        OwnDrops.Add(drop);
            });
        }

        public async Task<GetDropsResponse> GetDropsForMap(double mapNorth, double mapSouth, double mapWest, double mapEast)
        {
            var dropsResult = await _dropClient.GetDrops(mapNorth, mapSouth, mapWest, mapEast);
            foreach (var drop in dropsResult.Drops)
            {
                if (Drops.All(x => x.Id != drop.Id))
                {
                    Drops.Add(drop);
                }
            }
            return dropsResult;
        }

        public async Task<bool> DeleteDrop(Guid? dropId)
        {
            if (dropId.HasValue)
            {
                var resp = await _dropClient.DeleteDrop(dropId.Value, _deviceService.DeviceId);
                if (resp.IsSuccessStatusCode)
                {
                    //remove drop from cache of all drops
                    var dropToDelete = Drops.FirstOrDefault(x => x.Id == dropId);
                    if (dropToDelete != null)
                    {
                        Drops.Remove(dropToDelete);
                    }

                    //remove drop from cache of own drops
                    dropToDelete = OwnDrops.FirstOrDefault(x => x.Id == dropId);
                    if (dropToDelete != null)
                    {
                        OwnDrops.Remove(dropToDelete);
                    }
                }
                return resp.IsSuccessStatusCode;
            }
            return false;
        }

        public async Task<Drop> GetDrop(Guid? id)
        {
            if (id.HasValue)
            {
                var result = Drops.FirstOrDefault(x => x.Id == id);
                if (result == null)
                {
                    result = await _dropClient.GetDrop(id.Value);
                    if (result != null)
                        Drops.Add(result);
                }
                return result;
            }
            return null;
        }

        public async Task CreateDrop(Drop drop)
        {
            //set creatorId to current device id
            drop.CreatorId = _deviceService.DeviceId;

            var dropResult = await _dropClient.CreateDrop(drop);
            Drops.Add(dropResult);
            OwnDrops.Add(dropResult);
        }
    }
}
