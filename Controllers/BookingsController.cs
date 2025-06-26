using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineRentalSystem.Data;
using OnlineRentalSystem.Models.Identity;
using OnlineRentalSystem.Models.Rental;
using OnlineRentalSystem.ViewModels;

namespace OnlineRentalSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Bookings.Include(b => b.Booker).Include(b => b.Property);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else if(await _userManager.IsInRoleAsync(currentUser,"Buyer"))
            {
                return View(await applicationDbContext.Where(p => p.BookerId.Equals(currentUser.Id)).ToListAsync());
            }


            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Booker)
                .Include(b => b.Property)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        //public IActionResult Create()
        //{
        //    ViewData["BookerId"] = new SelectList(_context.Users, "Id", "UserName");
        //    ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title");
        //    return View();
        //}
        // GET: Bookings/Create?propertyId={id}
        public async Task<IActionResult> Create(Guid? propertyId)
        {
            if (propertyId == null)
            {
                // Redirect or show an error if no property ID is provided
                return RedirectToAction("Index", "Properties");
            }

            var property = await _context.Properties
                                         .Include(p => p.Images)
                                         .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

            if (property == null)
            {
                return NotFound(); // Property not found
            }

            // Populate the ViewModel for the view
            var model = new BookingViewModel
            {
                PropertyId = property.PropertyId,
                PropertyTitle = property.Title,
                PropertyDescription = property.Description,
                PropertyPricePerDay = property.PricePerDay,
                PropertyImageUrl = property.Images.FirstOrDefault()?.ImageUrl ?? "/images/placeholder-property.jpg", // Default placeholder
                StartDate = DateTime.Today.Date, // Suggest today as default
                EndDate = DateTime.Today.Date.AddDays(1) // Suggest tomorrow as default
            };

            return View(model);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            // Set TotalPrice in the model before validation if it's dependent on dates.
            // This is crucial if TotalPrice is required in the ViewModel or needs to be accurate for server-side validation.
            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate.Value >= model.StartDate.Value)
            {
                var diffTime = Math.Abs(model.EndDate.Value.Subtract(model.StartDate.Value).TotalDays);
                var diffDays = (int)Math.Ceiling(diffTime) + 1; // Add 1 for inclusive days
                model.TotalPrice = model.PropertyPricePerDay * diffDays;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid date range selected.");
                // If dates are invalid, fetch property details again to correctly display the view
                var property = await _context.Properties
                                             .Include(p => p.Images)
                                             .FirstOrDefaultAsync(p => p.PropertyId == model.PropertyId);
                if (property != null)
                {
                    model.PropertyTitle = property.Title;
                    model.PropertyDescription = property.Description;
                    model.PropertyPricePerDay = property.PricePerDay;
                    model.PropertyImageUrl = property.Images.FirstOrDefault()?.ImageUrl ?? "/images/placeholder-property.jpg";
                }
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found. Please log in.");
                    return View(model); // Or redirect to login
                }

                // Create the Booking entity
                var booking = new Booking
                {
                    PropertyId = model.PropertyId,
                    BookerId = currentUser.Id,
                    StartDate = model.StartDate.Value,
                    EndDate = model.EndDate.Value,
                    TotalPrice = model.TotalPrice, // Use the calculated total price
                    BookingDate = DateTime.UtcNow,
                    Status = "Pending" // Initial status
                };

                _context.Add(booking);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Booking created successfully for PropertyId: {model.PropertyId} by user: {currentUser.Email}");
                return RedirectToAction("BookingConfirmation", new { id = booking.BookingId }); // Redirect to a confirmation page
            }

            // If ModelState is not valid, re-populate display-only fields and return view
            // This is important because PropertyTitle, etc., are not bound from form but needed for display
            var propertyForInvalidModel = await _context.Properties
                                                 .Include(p => p.Images)
                                                 .FirstOrDefaultAsync(p => p.PropertyId == model.PropertyId);
            if (propertyForInvalidModel != null)
            {
                model.PropertyTitle = propertyForInvalidModel.Title;
                model.PropertyDescription = propertyForInvalidModel.Description;
                model.PropertyPricePerDay = propertyForInvalidModel.PricePerDay;
                model.PropertyImageUrl = propertyForInvalidModel.Images.FirstOrDefault()?.ImageUrl ?? "/images/placeholder-property.jpg";
            }

            return View(model);
        }

        // GET: Bookings/BookingConfirmation/{id} (Optional confirmation page)
        public async Task<IActionResult> BookingConfirmation(Guid id)
        {
            var booking = await _context.Bookings
                                        .Include(b => b.Property.Images)
                                        .Include(b => b.Booker)
                                        .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking); // You'd create a BookingConfirmation.cshtml view for this
        }
        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("BookingId,StartDate,EndDate,TotalPrice,Status,BookingDate,PropertyId,BookerId")] Booking booking)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(booking);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BookerId"] = new SelectList(_context.Users, "Id", "UserName", booking.BookerId);
        //    ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title", booking.PropertyId);
        //    return View(booking);
        //}

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["BookerId"] = new SelectList(_context.Users, "Id", "UserName", booking.BookerId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title", booking.PropertyId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BookingId,StartDate,EndDate,TotalPrice,Status,BookingDate,PropertyId,BookerId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["BookerId"] = new SelectList(_context.Users, "Id", "UserName", booking.BookerId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "Title", booking.PropertyId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Booker)
                .Include(b => b.Property)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(Guid id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
