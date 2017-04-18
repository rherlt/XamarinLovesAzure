using DropIt.Web.Client.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropIt.Web.Extensions
{
    public static class DropExtensions
    {
        public static Drop MapToApiDrop(this Models.Drop value)
        {
            var tmp = new Drop
            {
                Id = value.Id,
                CreatorId = value.CreatorId,
                ValidTo = value.ValidTo,
                IsValidForever = !value.ValidTo.HasValue,
                Lon = value.Lon ?? 0,
                Lat = value.Lat ?? 0,
                Alt = value.Alt ?? 0,
                Date = value.Date ?? new DateTime(),
                Message = value.Message,
                Title = value.Title,
            };

            return tmp;
        }
    }
}
