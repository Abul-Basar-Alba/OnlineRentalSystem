using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineRentalSystem.Data;
using OnlineRentalSystem.Models.Rental;

namespace OnlineRentalSystem.Controllers
{
    public class PropertyAmenitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertyAmenitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PropertyAmenities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PropertyAmenities.Include(p => p.Amenity).Include(p => p.Property);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PropertyAmenities/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAmenity = await _context.PropertyAmenities
                .Include(p => p.Amenity)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(m => m.PropertyAmenityId == id);
            if (propertyAmenity == null)
            {
                return NotFound();
            }

            return View(propertyAmenity);
        }

        // GET: PropertyAmenities/Create
        public IActionResult Create()
        {
            ViewData["AmenityId"] = new SelectList(_context.Amenities, "AmenityId", "Name");
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title");
            return View();
        }

        // POST: PropertyAmenities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyAmenityId,PropertyId,AmenityId,DateAdded")] PropertyAmenity propertyAmenity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyAmenity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AmenityId"] = new SelectList(_context.Amenities, "AmenityId", "Name", propertyAmenity.AmenityId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title", propertyAmenity.PropertyId);
            return View(propertyAmenity);
        }

        // GET: PropertyAmenities/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAmenity = await _context.PropertyAmenities.FindAsync(id);
            if (propertyAmenity == null)
            {
                return NotFound();
            }
            ViewData["AmenityId"] = new SelectList(_context.Amenities, "AmenityId", "Name", propertyAmenity.AmenityId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title", propertyAmenity.PropertyId);
            return View(propertyAmenity);
        }

        // POST: PropertyAmenities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PropertyAmenityId,PropertyId,AmenityId,DateAdded")] PropertyAmenity propertyAmenity)
        {
            if (id != propertyAmenity.PropertyAmenityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyAmenity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyAmenityExists(propertyAmenity.PropertyAmenityId))
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
            ViewData["AmenityId"] = new SelectList(_context.Amenities, "AmenityId", "Name", propertyAmenity.AmenityId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title", propertyAmenity.PropertyId);
            return View(propertyAmenity);
        }

        // GET: PropertyAmenities/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAmenity = await _context.PropertyAmenities
                .Include(p => p.Amenity)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(m => m.PropertyAmenityId == id);
            if (propertyAmenity == null)
            {
                return NotFound();
            }

            return View(propertyAmenity);
        }

        // POST: PropertyAmenities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyAmenity = await _context.PropertyAmenities.FindAsync(id);
            if (propertyAmenity != null)
            {
                _context.PropertyAmenities.Remove(propertyAmenity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyAmenityExists(Guid id)
        {
            return _context.PropertyAmenities.Any(e => e.PropertyAmenityId == id);
        }
    }
}
