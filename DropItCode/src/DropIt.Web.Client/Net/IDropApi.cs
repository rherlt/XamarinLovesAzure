using System;
using System.Net.Http;
using System.Threading.Tasks;
using DropIt.Web.Client.DataContracts;
using Refit;

namespace DropIt.Web.Client.Net
{
    public interface IDropApi
    {
        [Post("/api/drop/")]
        Task<Drop> CreateDrop([Body] Drop location);

        [Get("/api/drop/{id}")]
        Task<Drop> GetDrop(Guid? id);

        [Get("/api/drop/?latN={latN}&latS={latS}&lonE={lonE}&lonW={lonW}")]
        Task<GetDropsResponse> GetDrops(double? latN, double? latS, double? lonE, double? lonW);

        [Delete("/api/drop/?id={id}&creatorId={creatorId}")]
        Task<HttpResponseMessage> DeleteDrop(Guid? id, Guid? creatorId);

        [Get("/api/owndrop/{creatorId}")]
        Task<GetDropsResponse> GetOwnDrops(Guid? creatorId);
    }
}
