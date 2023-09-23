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
    public class RatingsController : Controller
    {
        private readonly RestaurantDbContext _context;

        public RatingsController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: Ratings
        public async Task<IActionResult> Index()
        {
            var restaurantDbContext = _context.Ratings.Include(r => r.Client).Include(r => r.Place);
            return View(await restaurantDbContext.ToListAsync());
        }

        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ratings == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Client)
                .Include(r => r.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Ratings/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = _context.Clients.ToList();
            ViewData["PlaceId"] = _context.Places.ToList();
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string ClientName, string PlaceName, [Bind("Id,PlaceId,Score")] Rating rating)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Name == ClientName);
            var place = _context.Places.FirstOrDefault(p => p.Name == PlaceName);
            if(client != null && place != null)
            {
                rating.ClientId = client.Id;
                rating.PlaceId = place.Id;
                if (ModelState.IsValid)
                {
                    _context.Add(rating);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ClientId"] = _context.Clients.ToList();
            ViewData["PlaceId"] = _context.Places.ToList();
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ratings == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = _context.Clients.ToList();
            ViewData["PlaceId"] = _context.Places.ToList();
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string ClientName, string PlaceName, [Bind("Id,PlaceId,Score")] Rating rating)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Name == ClientName);
            var place = _context.Places.FirstOrDefault(p => p.Name == PlaceName);
            if (id != rating.Id)
            {
                return NotFound();
            }
            if (client != null && place != null)
            {
                rating.ClientId = client.Id;
                rating.PlaceId = place.Id;
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(rating);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RatingExists(rating.Id))
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
            ViewData["ClientId"] = _context.Clients.ToList();
            ViewData["PlaceId"] = _context.Places.ToList();
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ratings == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Client)
                .Include(r => r.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ratings == null)
            {
                return Problem("Entity set 'RestaurantDbContext.Ratings'  is null.");
            }
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
          return (_context.Ratings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
