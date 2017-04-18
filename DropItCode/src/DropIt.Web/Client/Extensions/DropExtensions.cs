using System;
using DropIt.Web.Client.DataContracts;

namespace DropIt.Web.Client.Extensions
{
    public static class DropExtensions
    {
        public static bool Validate(this Drop item)
        {
            var isGood = IsPresent(item.CreatorId) &&
                         IsPresent(item.Message) &&
                         IsPresent(item.Title) &&
                         IsPresent(item.Lon) &&
                         IsPresent(item.Lat) &&
                         IsPresent(item.Date);
            return isGood;
        }
        
        public static bool IsPresent(this object o)
        {
            return o != null && !string.IsNullOrWhiteSpace(Convert.ToString(o));
        }
    }
}
