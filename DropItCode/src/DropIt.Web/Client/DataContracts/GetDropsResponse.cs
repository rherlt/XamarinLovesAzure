using System.Collections.Generic;
using Newtonsoft.Json;

namespace DropIt.Web.Client.DataContracts
{
    public class GetDropsResponse
    {
        public GetDropsResponse()
        {
            Drops = new List<Drop>();
        }

        [JsonProperty("drops")]
        public List<Drop> Drops { get; set; }
    }
}