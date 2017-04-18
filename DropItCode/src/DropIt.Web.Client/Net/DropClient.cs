using System;
using System.Net.Http;
using System.Threading.Tasks;
using DropIt.Web.Client.DataContracts;
using Refit;

namespace DropIt.Web.Client.Net
{
	public class DropClient
	{
		private readonly IDropApi _api;

		public DropClient(HttpClient httpClient)
		{

			_api = RestService.For<IDropApi>(httpClient);
		}

		public DropClient(string hostUrl)
		{
			_api = RestService.For<IDropApi>(hostUrl);
		}


		public Task<Drop> CreateDrop(Drop drop)
		{
			return _api.CreateDrop(drop);
		}


        public Task<GetDropsResponse> GetOwnDrops(Guid? creatorId)
		{
			return _api.GetOwnDrops(creatorId);
		}

		public Task<GetDropsResponse> GetDrops(double? latN, double? latS, double? lonE, double? lonW)
		{
			return _api.GetDrops(latN, latS, lonE, lonW);
		}

		public Task<Drop> GetDrop(Guid id)
		{
			return _api.GetDrop(id);
		}


		[Delete("/api/drop/{id}")]
		public Task<HttpResponseMessage> DeleteDrop(Guid id, Guid creatorId)
		{
			return _api.DeleteDrop(id, creatorId);
		}

	}
}
