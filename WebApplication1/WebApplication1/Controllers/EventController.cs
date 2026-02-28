using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.Include(e => e.Venue).ToListAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueID");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var events = await _context.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null)
            {
                return View("Error");
            }
            return View(events);
                
            
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var events = await _context.Event.Include(e => e.Venue).FirstOrDefaultAsync(m => m.EventID == id);
            if (events == null)
            {
                return View("Error");
            }
            return View(events);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
        var events = await _context.Event.FindAsync(id);
            if (events == null)
            {
                return View("Error");
            }
            var booking = _context.Booking.Where(e => e.EventID == id);
            if (booking == null)
            {
                return View("Error");
            }
            _context.Booking.RemoveRange(booking);
            _context.Event.Remove(events);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var events = await _context.Event.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueID", events.VenueID);
            return View(events);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,Event events)
        {
            if (id != events.EventID)
            {
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var booking = await _context.Booking.FirstOrDefaultAsync(e => e.EventID == events.EventID);
                    if (booking != null)
                    {
                        booking.VenueID = (int)events.VenueID;
                        _context.Update(booking);
                    }
                    
                    _context.Update(events);
                    
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(events.EventID))
                    {
                        return NotFound();
                    }else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "VenueID", events.VenueID);
            return View(events);
        }
    }
}
