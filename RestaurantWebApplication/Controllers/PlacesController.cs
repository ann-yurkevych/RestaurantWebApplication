using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantWebApplication.Models;

namespace RestaurantWebApplication.Controllers
{
    public class PlacesController : Controller
    {
        private readonly RestaurantDbContext _context;

        public PlacesController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: Places
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null || name == null)
            {
                return RedirectToAction("Index", "Types");
            }
            ViewBag.TypeId = id;
            ViewBag.TypeName = name;
            var restaurantDbContext = _context.Places.Where(r => r.TypeId == id);
            return View(await restaurantDbContext.ToListAsync());
        }

        // GET: Places/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Places == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            ViewData["ClientRatings"] = _context.Ratings.Where(rating => rating.PlaceId == id).Include(rating => rating.Client).ToList();
            return View(place);
        }

        // GET: Places/Create
        public IActionResult Create(int id)
        {
            ViewBag.TypeId = id;
            ViewBag.TypeName = _context.Types.First(t => t.Id == id).Name;
            return View();
        }

        // POST: Places/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int TypeId, [Bind("Name,AverageBill,OpenTime,CloseTime,Location")] Place place)
        {
            place.TypeId = TypeId;
            if (ModelState.IsValid)
            {
                _context.Add(place);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = TypeId, name = _context.Types.First(t => t.Id == TypeId).Name });
            }
            ViewBag.TypeId = TypeId;
            ViewBag.TypeName = _context.Types.First(t => t.Id == TypeId).Name;
            return View(place);
        }

        // GET: Places/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Places == null)
            {
                return NotFound();
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            ViewData["TypeId"] = _context.Types.ToList();
            return View(place);
        }

        // POST: Places/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string TypeName, [Bind("Id,Name,AverageBill,OpenTime,CloseTime,Location")] Place place)
        {
            var type = _context.Types.FirstOrDefault(t => t.Name == TypeName);
            if (id != place.Id)
            {
                return NotFound();
            }
            if(type != null)
            {
                place.TypeId = type.Id;
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(place);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PlaceExists(place.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["TypeId"] = _context.Types.ToList();
            return View(place);
        }

        // GET: Places/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Places == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Places == null)
            {
                return Problem("Entity set 'RestaurantDbContext.Places'  is null.");
            }
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                _context.Places.Remove(place);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaceExists(int id)
        {
          return (_context.Places?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
