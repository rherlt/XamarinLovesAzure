using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyChain.Net;
using Splat;

namespace DropIt.Mobile.Core.Services
{
    public class DeviceService : IDeviceService
    {
        public DeviceService()
        {
            var keychainHelper = Locator.Current.GetService<IKeyChainHelper>();
            var deviceId = keychainHelper.GetKey(Const.DeviceIdKey);
            if (string.IsNullOrEmpty(deviceId))
            {
                DeviceId = Guid.NewGuid();
                deviceId = DeviceId.ToString();
                keychainHelper.SetKey(Const.DeviceIdKey, deviceId);
            }
            else
            {
                DeviceId = new Guid(deviceId);
            }
        }

        public Guid DeviceId { get; protected set; }
    }
}
