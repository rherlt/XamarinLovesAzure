using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DropIt.Web.Data;
using DropIt.Web.Models;

namespace DropIt.Web.Controllers
{
    public class DropsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DropsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Drops
        public async Task<IActionResult> Index()
        {
            return View(await _context.Drops.ToListAsync());
        }

        // GET: Drops/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drop = await _context.Drops
                .SingleOrDefaultAsync(m => m.Id == id);
            if (drop == null)
            {
                return NotFound();
            }

            return View(drop);
        }

        // GET: Drops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drops/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreatorId,Lon,Lat,Alt,Date,ValidTo,IsValidForever,Title,Message,Id")] Drop drop)
        {
            if (ModelState.IsValid)
            {
                drop.Id = Guid.NewGuid();
                _context.Add(drop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(drop);
        }

        // GET: Drops/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drop = await _context.Drops.SingleOrDefaultAsync(m => m.Id == id);
            if (drop == null)
            {
                return NotFound();
            }
            return View(drop);
        }

        // POST: Drops/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CreatorId,Lon,Lat,Alt,Date,ValidTo,IsValidForever,Title,Message,Id")] Drop drop)
        {
            if (id != drop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DropExists(drop.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(drop);
        }

        // GET: Drops/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drop = await _context.Drops
                .SingleOrDefaultAsync(m => m.Id == id);
            if (drop == null)
            {
                return NotFound();
            }

            return View(drop);
        }

        // POST: Drops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var drop = await _context.Drops.SingleOrDefaultAsync(m => m.Id == id);
            _context.Drops.Remove(drop);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool DropExists(Guid id)
        {
            return _context.Drops.Any(e => e.Id == id);
        }
    }
}
