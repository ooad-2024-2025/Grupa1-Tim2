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
    public class OcjenaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OcjenaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ocjena
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ocjena.Include(o => o.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ocjena/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocjena = await _context.Ocjena
                .Include(o => o.Korisnik)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ocjena == null)
            {
                return NotFound();
            }

            return View(ocjena);
        }

        // GET: Ocjena/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Ocjena/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ocjena,komentar,KorisnikId")] Ocjena ocjena)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ocjena);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id", ocjena.KorisnikId);
            return View(ocjena);
        }

        // GET: Ocjena/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocjena = await _context.Ocjena.FindAsync(id);
            if (ocjena == null)
            {
                return NotFound();
            }
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id", ocjena.KorisnikId);
            return View(ocjena);
        }

        // POST: Ocjena/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ocjena,komentar,KorisnikId")] Ocjena ocjena)
        {
            if (id != ocjena.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ocjena);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OcjenaExists(ocjena.ID))
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
            ViewData["KorisnikId"] = new SelectList(_context.Users, "Id", "Id", ocjena.KorisnikId);
            return View(ocjena);
        }

        // GET: Ocjena/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocjena = await _context.Ocjena
                .Include(o => o.Korisnik)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ocjena == null)
            {
                return NotFound();
            }

            return View(ocjena);
        }

        // POST: Ocjena/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ocjena = await _context.Ocjena.FindAsync(id);
            if (ocjena != null)
            {
                _context.Ocjena.Remove(ocjena);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OcjenaExists(int id)
        {
            return _context.Ocjena.Any(e => e.ID == id);
        }

        [HttpPost]
        [Route("api/ocjena/create")]
        public async Task<IActionResult> CreateApi([FromBody] Ocjena novaOcjena)
        {
            if (ModelState.IsValid)
            {
                _context.Ocjena.Add(novaOcjena);
                await _context.SaveChangesAsync();
                return Ok();
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }


    }
}
