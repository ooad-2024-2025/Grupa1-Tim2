using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YourRide.Data;
using YourRide.Models;

namespace YourRide.Controllers
{
    public class NotifikacijaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotifikacijaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notifikacija
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Notifikacija.Include(n => n.Posiljalac).Include(n => n.Primalac);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Notifikacija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notifikacija = await _context.Notifikacija
                .Include(n => n.Posiljalac)
                .Include(n => n.Primalac)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (notifikacija == null)
            {
                return NotFound();
            }

            return View(notifikacija);
        }

        // GET: Notifikacija/Create
        [Authorize(Roles = "Administrator,Putnik,Vozac")]
        public IActionResult Create()
        {
            ViewData["PosiljalacId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["PrimalacId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Notifikacija/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,poruka,VrijemeSlanja,PosiljalacId,PrimalacId")] Notifikacija notifikacija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notifikacija);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PosiljalacId"] = new SelectList(_context.Users, "Id", "Id", notifikacija.PosiljalacId);
            ViewData["PrimalacId"] = new SelectList(_context.Users, "Id", "Id", notifikacija.PrimalacId);
            return View(notifikacija);
        }

        // GET: Notifikacija/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notifikacija = await _context.Notifikacija.FindAsync(id);
            if (notifikacija == null)
            {
                return NotFound();
            }
            ViewData["PosiljalacId"] = new SelectList(_context.Users, "Id", "Id", notifikacija.PosiljalacId);
            ViewData["PrimalacId"] = new SelectList(_context.Users, "Id", "Id", notifikacija.PrimalacId);
            return View(notifikacija);
        }

        // POST: Notifikacija/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,poruka,VrijemeSlanja,PosiljalacId,PrimalacId")] Notifikacija notifikacija)
        {
            if (id != notifikacija.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notifikacija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotifikacijaExists(notifikacija.ID))
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
            ViewData["PosiljalacId"] = new SelectList(_context.Users, "Id", "Id", notifikacija.PosiljalacId);
            ViewData["PrimalacId"] = new SelectList(_context.Users, "Id", "Id", notifikacija.PrimalacId);
            return View(notifikacija);
        }

        // GET: Notifikacija/Delete/5
        [Authorize(Roles = "Administrator,Putnik,Vozac")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notifikacija = await _context.Notifikacija
                .Include(n => n.Posiljalac)
                .Include(n => n.Primalac)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (notifikacija == null)
            {
                return NotFound();
            }

            return View(notifikacija);
        }

        // POST: Notifikacija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notifikacija = await _context.Notifikacija.FindAsync(id);
            if (notifikacija != null)
            {
                _context.Notifikacija.Remove(notifikacija);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotifikacijaExists(int id)
        {
            return _context.Notifikacija.Any(e => e.ID == id);
        }
    }
}
