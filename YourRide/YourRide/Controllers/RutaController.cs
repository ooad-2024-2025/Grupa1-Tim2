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
    public class RutaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RutaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ruta
        [Authorize(Roles = "Administrator,Vozac,TehnickaPodrska")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ruta.Include(r => r.KrajnjaLokacija).Include(r => r.PocetnaLokacija);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ruta/Details/5
        [Authorize(Roles = "Administrator,Vozac,TehnickaPodrska")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ruta = await _context.Ruta
                .Include(r => r.KrajnjaLokacija)
                .Include(r => r.PocetnaLokacija)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ruta == null)
            {
                return NotFound();
            }

            return View(ruta);
        }

        // GET: Ruta/Create
        [Authorize(Roles = "Nepostojeci")]
        public IActionResult Create()
        {
            ViewData["KrajnjaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID");
            ViewData["PocetnaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID");
            return View();
        }

        // POST: Ruta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,PocetnaLokacijaId,KrajnjaLokacijaId")] Ruta ruta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ruta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KrajnjaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID", ruta.KrajnjaLokacijaId);
            ViewData["PocetnaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID", ruta.PocetnaLokacijaId);
            return View(ruta);
        }

        // GET: Ruta/Edit/5
        [Authorize(Roles = "Nepostojeci")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ruta = await _context.Ruta.FindAsync(id);
            if (ruta == null)
            {
                return NotFound();
            }
            ViewData["KrajnjaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID", ruta.KrajnjaLokacijaId);
            ViewData["PocetnaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID", ruta.PocetnaLokacijaId);
            return View(ruta);
        }

        // POST: Ruta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,PocetnaLokacijaId,KrajnjaLokacijaId")] Ruta ruta)
        {
            if (id != ruta.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ruta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RutaExists(ruta.ID))
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
            ViewData["KrajnjaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID", ruta.KrajnjaLokacijaId);
            ViewData["PocetnaLokacijaId"] = new SelectList(_context.Lokacija, "ID", "ID", ruta.PocetnaLokacijaId);
            return View(ruta);
        }

        // GET: Ruta/Delete/5
        [Authorize(Roles = "Nepostojeci")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ruta = await _context.Ruta
                .Include(r => r.KrajnjaLokacija)
                .Include(r => r.PocetnaLokacija)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ruta == null)
            {
                return NotFound();
            }

            return View(ruta);
        }

        // POST: Ruta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ruta = await _context.Ruta.FindAsync(id);
            if (ruta != null)
            {
                _context.Ruta.Remove(ruta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RutaExists(int id)
        {
            return _context.Ruta.Any(e => e.ID == id);
        }
    }
}
