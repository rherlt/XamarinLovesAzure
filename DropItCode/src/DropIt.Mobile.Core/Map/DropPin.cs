using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropIt.Web.Client.DataContracts;
using Xamarin.Forms.Maps;

namespace DropIt.Mobile.Core.Map
{
    public class DropPin
    {
        public DropPin(Drop drop, Pin pin)
        {
            Drop = drop;
            Pin = pin;
        }

        public Drop Drop { get; set; }     
        
        public Pin Pin { get; set; }

        public object PlatformMarker { get; set; }
    }
}
