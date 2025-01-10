using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;

namespace gestionPharmacieApp.Controllers
{
    public class ProgFidelitesController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public ProgFidelitesController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: ProgFidelites
        public async Task<IActionResult> Index()
        {
            var gestionPharmacieBdContext = _context.ProgFidelites.Include(p => p.IdClientNavigation);
            return View(await gestionPharmacieBdContext.ToListAsync());
        }

        // GET: ProgFidelites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progFidelite = await _context.ProgFidelites
                .Include(p => p.IdClientNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (progFidelite == null)
            {
                return NotFound();
            }

            return View(progFidelite);
        }

        // GET: ProgFidelites/Create
        public IActionResult Create()
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient");
            return View();
        }

        // POST: ProgFidelites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Points,IdClient,Remise")] ProgFidelite progFidelite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(progFidelite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", progFidelite.IdClient);
            return View(progFidelite);
        }

        // GET: ProgFidelites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progFidelite = await _context.ProgFidelites.FindAsync(id);
            if (progFidelite == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", progFidelite.IdClient);
            return View(progFidelite);
        }

        // POST: ProgFidelites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Points,IdClient,Remise")] ProgFidelite progFidelite)
        {
            if (id != progFidelite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(progFidelite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgFideliteExists(progFidelite.Id))
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
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", progFidelite.IdClient);
            return View(progFidelite);
        }

        // GET: ProgFidelites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progFidelite = await _context.ProgFidelites
                .Include(p => p.IdClientNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (progFidelite == null)
            {
                return NotFound();
            }

            return View(progFidelite);
        }

        // POST: ProgFidelites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var progFidelite = await _context.ProgFidelites.FindAsync(id);
            if (progFidelite != null)
            {
                _context.ProgFidelites.Remove(progFidelite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProgFideliteExists(int id)
        {
            return _context.ProgFidelites.Any(e => e.Id == id);
        }
    }
}
