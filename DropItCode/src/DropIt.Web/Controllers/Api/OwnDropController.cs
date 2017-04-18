using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DropIt.Web.Client.DataContracts;
using DropIt.Web.Client.Extensions;
using DropIt.Web.Data;
using DropIt.Web.Extensions;
using DropIt.Web.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DropIt.Web.Controllers.Api
{
   [Route("api/OwnDrop")]
    public class OwnDropController : ApiHubController<DropHub>
    {
        private readonly ApplicationDbContext _dbContext;

        public OwnDropController(ApplicationDbContext dbContext, IConnectionManager signalRConnectionManager) : base (signalRConnectionManager)
        {
            _dbContext = dbContext;
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid? id)
        {
            try
            {
                if (!ModelState.IsValid || !id.HasValue)
                {
                    return BadRequest(ModelState);
                }

                var drops = await _dbContext.Drops.Where(x => x.CreatorId == id).ToListAsync();

                var resp = new GetDropsResponse();
                foreach (var drop in drops)
                    resp.Drops.Add(drop.MapToApiDrop());

                return Ok(resp);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                throw;
            }
        }
    }
}
