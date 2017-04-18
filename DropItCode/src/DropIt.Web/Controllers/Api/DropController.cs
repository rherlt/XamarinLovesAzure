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
   [Route("api/Drop")]
    public class DropController : ApiHubController<DropHub>
    {
        private readonly ApplicationDbContext _dbContext;
        public const string GetDropByIdRoute = "GetDropById";

        public DropController(ApplicationDbContext dbContext, IConnectionManager signalRConnectionManager) : base (signalRConnectionManager)
        {
            _dbContext = dbContext;
        }

        // GET: api/Location
        [HttpGet]
        public async Task<IActionResult> Get(double? latN, double? latS, double? lonE, double? lonW)
        {
            try
            {
                if (! (latN.IsPresent() && latS.IsPresent() && lonE.IsPresent() && lonW.IsPresent()))
                    return BadRequest();

                var drops = await _dbContext.Drops.Where(x => 
                    x.Lat <= latN &&
                    x.Lat >= latS &&
                    x.Lon >= lonE &&
                    x.Lon <= lonW).ToListAsync();

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

        [HttpGet("{id}", Name = GetDropByIdRoute)]
        public async Task<IActionResult> Get(Guid? id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var drop = await _dbContext.Drops.FirstOrDefaultAsync(x => x.Id == id);
                if (drop == null)
                    return NotFound();

                var resp = drop.MapToApiDrop();
                return Ok(resp);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Drop value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!value.Validate())
            {
                return BadRequest();
            }

            var drop = new Models.Drop()
            {
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

            try
            {
                //add record to db set
                _dbContext.Drops.Add(drop);
                await _dbContext.SaveChangesAsync();

                //update Id
                value.Id = drop.Id;

                //first notify clients about new position
                await Task.Run(() => Clients.All.NotifyNewDrop(value)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            return CreatedAtRoute(routeName:GetDropByIdRoute, routeValues: new { value.Id}, value:value);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]Guid? id, [FromQuery]Guid? creatorId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var drop = await _dbContext.Drops.FirstOrDefaultAsync(x => x.Id == id && creatorId == x.CreatorId);
                if (drop == null)
                    return NotFound();

                _dbContext.Drops.Remove(drop);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                throw;
            }
        }
    }
}
