using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YourRide.Data;
using YourRide.Models;

namespace YourRide.Controllers
{
    public class VoznjaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VoznjaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Voznja
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Voznja.Include(v => v.Putnik).Include(v => v.Ruta).Include(v => v.Vozac);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Voznja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voznja = await _context.Voznja
                .Include(v => v.Putnik)
                .Include(v => v.Ruta)
                .Include(v => v.Vozac)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (voznja == null)
            {
                return NotFound();
            }

            return View(voznja);
        }

        // GET: Voznja/Create
        public IActionResult Create()
        {
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID");
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Voznja/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,status,PutnikId,VozacId,RutaId,VrijemePocetka,VrijemeZavrsetka,Cijena")] Voznja voznja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(voznja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id", voznja.PutnikId);
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID", voznja.RutaId);
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id", voznja.VozacId);
            return View(voznja);
        }

        // GET: Voznja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voznja = await _context.Voznja.FindAsync(id);
            if (voznja == null)
            {
                return NotFound();
            }
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id", voznja.PutnikId);
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID", voznja.RutaId);
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id", voznja.VozacId);
            return View(voznja);
        }

        // POST: Voznja/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,status,PutnikId,VozacId,RutaId,VrijemePocetka,VrijemeZavrsetka,Cijena")] Voznja voznja)
        {
            if (id != voznja.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voznja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoznjaExists(voznja.ID))
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
            ViewData["PutnikId"] = new SelectList(_context.Users, "Id", "Id", voznja.PutnikId);
            ViewData["RutaId"] = new SelectList(_context.Ruta, "ID", "ID", voznja.RutaId);
            ViewData["VozacId"] = new SelectList(_context.Users, "Id", "Id", voznja.VozacId);
            return View(voznja);
        }

        // GET: Voznja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voznja = await _context.Voznja
                .Include(v => v.Putnik)
                .Include(v => v.Ruta)
                .Include(v => v.Vozac)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (voznja == null)
            {
                return NotFound();
            }

            return View(voznja);
        }

        // POST: Voznja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voznja = await _context.Voznja.FindAsync(id);
            if (voznja != null)
            {
                _context.Voznja.Remove(voznja);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoznjaExists(int id)
        {
            return _context.Voznja.Any(e => e.ID == id);
        }
    }
}
