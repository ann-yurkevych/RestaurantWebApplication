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
    public class FavouritesController : Controller
    {
        private readonly RestaurantDbContext _context;

        public FavouritesController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: Favourites
        public async Task<IActionResult> Index()
        {
            var restaurantDbContext = _context.Favourites.Include(f => f.Client).Include(f => f.Place);
            return View(await restaurantDbContext.ToListAsync());
        }

        // GET: Favourites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Favourites == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites
                .Include(f => f.Client)
                .Include(f => f.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favourite == null)
            {
                return NotFound();
            }

            return View(favourite);
        }

        // GET: Favourites/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = _context.Clients.ToList();
            ViewData["PlaceId"] = _context.Places.ToList();
            return View();
        }

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string ClientName, string PlaceName, [Bind("PlaceId,ClientId")] Favourite favourite)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Name == ClientName);
            var place = _context.Places.FirstOrDefault(p => p.Name == PlaceName);
            if (client != null && place != null)
            {
                favourite.ClientId = client.Id;
                favourite.PlaceId = place.Id;
                if (ModelState.IsValid)
                {
                    _context.Add(favourite);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ClientId"] = _context.Clients.ToList();
            ViewData["PlaceId"] = _context.Places.ToList();
            return View(favourite);
        }

        // GET: Favourites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Favourites == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = _context.Clients.ToList();
            ViewData["PlaceId"] = _context.Places.ToList();
            return View(favourite);
        }

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string ClientName, string PlaceName, [Bind("Id,PlaceId,ClientId")] Favourite favourite)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Name == ClientName);
            var place = _context.Places.FirstOrDefault(p => p.Name == PlaceName);
            if (id != favourite.Id)
            {
                return NotFound();
            }

            if (client != null && place != null)
            {
                favourite.ClientId = client.Id;
                favourite.PlaceId = place.Id;
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(favourite);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FavouriteExists(favourite.Id))
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
            return View(favourite);
        }

        // GET: Favourites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Favourites == null)
            {
                return NotFound();
            }

            var favourite = await _context.Favourites
                .Include(f => f.Client)
                .Include(f => f.Place)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favourite == null)
            {
                return NotFound();
            }

            return View(favourite);
        }

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Favourites == null)
            {
                return Problem("Entity set 'RestaurantDbContext.Favourites'  is null.");
            }
            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite != null)
            {
                _context.Favourites.Remove(favourite);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavouriteExists(int id)
        {
          return (_context.Favourites?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
