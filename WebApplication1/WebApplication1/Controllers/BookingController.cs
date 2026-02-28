using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var booking = await _context.Booking.Include(e => e.Event).ThenInclude(c => c.Venue).ToListAsync();
            return View(booking);
        }
        public IActionResult Create()
        {
            ViewBag.EventID = new SelectList(_context.Event, "EventID", "EventID");
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueID");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var tempvenue = _context.Event.Where(e => e.EventID == booking.EventID);
                booking.VenueID = (int)tempvenue.ElementAt(0).VenueID;
                Console.WriteLine("This is the tempvenue" + tempvenue);
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var booking = await _context.Booking
                .Include(e => e.Event)
                .ThenInclude(c => c.Venue)
                .FirstOrDefaultAsync(m => m.BookingID== id);

            if (booking == null)
            {
                return View("Error");
            }
            return View(booking);


        }
        public async Task<IActionResult> Delete(int? id)
        {
            var booking = await _context.Booking.Include(e => e.Event).ThenInclude(c => c.Venue).FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return View("Error");
            }
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return View("Error");
            }
            
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewBag.EventID = new SelectList(_context.Event, "EventID", "EventID", booking.EventID);
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueID", booking.VenueID);
            return View(booking);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID)
            {
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var tempvenue = _context.Event.Where(e => e.EventID == booking.EventID);
                    booking.VenueID = (int)tempvenue.ElementAt(0).VenueID;
                    _context.Update(booking);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
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
            ViewBag.EventID = new SelectList(_context.Event, "EventID", "EventID", booking.EventID);
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueID", booking.VenueID);
            return View(booking);
        }
    }
}
