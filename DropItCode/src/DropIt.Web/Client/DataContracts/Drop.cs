using System;
using Newtonsoft.Json;

namespace DropIt.Web.Client.DataContracts
{
    public class Drop : IIdModel
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        [JsonProperty("creatorId")]
        public virtual Guid? CreatorId { get; set; }

		[JsonProperty("lat")]
		public virtual double? Lat { get; set; }

        [JsonProperty("lon")]
        public virtual double? Lon { get; set; }

        [JsonProperty("alt")]
        public virtual double? Alt { get; set; }

        [JsonProperty("date")]
        public virtual DateTime? Date { get; set; }

        [JsonProperty("validTo")]
        public virtual DateTime? ValidTo { get; set; }

        [JsonProperty("isValidForever")]
        public virtual bool? IsValidForever { get; set; }

        [JsonProperty("title")]
        public virtual string Title { get; set; }

        [JsonProperty("message")]
        public virtual string Message { get; set; }

    }
}