using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventeaseP7.Models;


namespace EventeaseP7.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchString)
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookingsQuery = bookingsQuery.Where(b => b.Event.EventName.Contains(searchString));
            }

            var bookings = await bookingsQuery.ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName");
            ViewBag.Venue_ID = new SelectList(_context.Venues, "Venue_ID", "Venue_Name");
            return View();
        }



        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewBag.VenueID = new SelectList(_context.Venues, "Venue_ID", "Venue_Name", booking.Venue_ID);

            var selectedEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event cannot be found.");
                //  ViewData["Events"] = _context.Events.ToList();
                //  ViewData["Venues"] = _context.Venues.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                var conflict = await _context.Bookings
                .AnyAsync(b => b.Venue_ID == booking.Venue_ID &&
                                b.BookingDate.Date == booking.BookingDate.Date);


                if (conflict)
                {
                    ModelState.AddModelError("", "The selected venue is already booked for the event date.");
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }





        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewBag.Venue_ID = new SelectList(_context.Venues, "Venue_ID", "Venue_Name", booking.Venue_ID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(e => e.BookingID == booking.BookingID))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.EventID = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewBag.Venue_ID = new SelectList(_context.Venues, "Venue_ID", "VenueName", booking.Venue_ID);

            return View(booking);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var events = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null)
                return NotFound();

            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var events = await _context.Events.FindAsync(id);
            if (events == null)
                return NotFound();
            try
            {
                _context.Events.Remove(events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("FK_Bookings_EventI") == true)
                {
                    TempData["ErrorMessage"] = "You can't delete this event because it has existing bookings.";
                }
                else
                {
                    TempData["ErrorMessage"] = "An unexpected error occurred while trying to delete the event.";
                }

                return RedirectToAction(nameof(Index));
            }
        }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}