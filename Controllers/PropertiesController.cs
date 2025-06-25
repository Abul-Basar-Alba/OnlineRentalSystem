using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineRentalSystem.Data;
using OnlineRentalSystem.Models.Rental;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineRentalSystem.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyType)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["PropertyTypeId"] = new SelectList(_context.PropertyTypes, "PropertyTypeId", "Name");
            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyId,Title,Description,PricePerDay,Address,City,StateOrProvince,Country,ZipCode,IsAvailable,DateAdded,PropertyTypeId,OwnerId,ReferenceID")] Property property)
        {
            if (ModelState.IsValid)
            {
                property.PropertyId = Guid.NewGuid();
                _context.Add(property);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName", property.OwnerId);
            ViewData["PropertyTypeId"] = new SelectList(_context.PropertyTypes, "PropertyTypeId", "Name", property.PropertyTypeId);
            return View(property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName", property.OwnerId);
            ViewData["PropertyTypeId"] = new SelectList(_context.PropertyTypes, "PropertyTypeId", "Name", property.PropertyTypeId);
            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PropertyId,Title,Description,PricePerDay,Address,City,StateOrProvince,Country,ZipCode,IsAvailable,DateAdded,PropertyTypeId,OwnerId,ReferenceID")] Property property)
        {
            if (id != property.PropertyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(property.PropertyId))
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
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName", property.OwnerId);
            ViewData["PropertyTypeId"] = new SelectList(_context.PropertyTypes, "PropertyTypeId", "Name", property.PropertyTypeId);
            return View(property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyType)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id) // Fixed from int to Guid
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(Guid id)
        {
            return _context.Properties.Any(e => e.PropertyId == id);
        }
    }
}