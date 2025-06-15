using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourRide.Data;
using YourRide.Models;

namespace YourRide.Controllers
{
    public class PodrskaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PodrskaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Proba
        [Authorize(Roles = "Administrator,TehnickaPodrska")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PorukePodrske.Include(p => p.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Proba/Details/5
        [Authorize(Roles = "Administrator,TehnickaPodrska")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podrskaPoruka = await _context.PorukePodrske
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (podrskaPoruka == null)
            {
                return NotFound();
            }

            return View(podrskaPoruka);
        }

        // GET: Proba/Create
        [Authorize(Roles = "Nepostojeci")]
        public IActionResult Create()
        {
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Proba/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Naslov,Poruka,DatumSlanja,KorisnikId")] PodrskaPoruka podrskaPoruka)
        {
            if (ModelState.IsValid)
            {
                _context.Add(podrskaPoruka);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id", podrskaPoruka.KorisnikId);
            return View(podrskaPoruka);
        }

        // GET: Proba/Edit/5

        [Authorize(Roles = "Nepostojeći")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podrskaPoruka = await _context.PorukePodrske.FindAsync(id);
            if (podrskaPoruka == null)
            {
                return NotFound();
            }
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id", podrskaPoruka.KorisnikId);
            return View(podrskaPoruka);
        }

        // POST: Proba/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Naslov,Poruka,DatumSlanja,KorisnikId")] PodrskaPoruka podrskaPoruka)
        {
            if (id != podrskaPoruka.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(podrskaPoruka);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PodrskaPorukaExists(podrskaPoruka.ID))
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
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id", podrskaPoruka.KorisnikId);
            return View(podrskaPoruka);
        }

        // GET: Proba/Delete/5
        [Authorize(Roles = "TehnickaPodrska")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var podrskaPoruka = await _context.PorukePodrske
                .Include(p => p.Korisnik)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (podrskaPoruka == null)
            {
                return NotFound();
            }

            return View(podrskaPoruka);
        }

        // POST: Proba/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var podrskaPoruka = await _context.PorukePodrske.FindAsync(id);
            if (podrskaPoruka != null)
            {
                _context.PorukePodrske.Remove(podrskaPoruka);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PodrskaPorukaExists(int id)
        {
            return _context.PorukePodrske.Any(e => e.ID == id);
        }
    }
}
