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
    public class VentesController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public VentesController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: Ventes
        public async Task<IActionResult> Index()
        {
            var gestionPharmacieBdContext = _context.Ventes.Include(v => v.IdClientNavigation).Include(v => v.ReferenceNavigation);
            return View(await gestionPharmacieBdContext.ToListAsync());
        }

        // GET: Ventes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _context.Ventes
                .Include(v => v.IdClientNavigation)
                .Include(v => v.ReferenceNavigation)
                .FirstOrDefaultAsync(m => m.IdVente == id);
            if (vente == null)
            {
                return NotFound();
            }

            return View(vente);
        }

        // GET: Ventes/Create
        public IActionResult Create()
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient");
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference");
            return View();
        }

        // POST: Ventes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVente,DateVente,Reference,Quantite,IdClient")] Vente vente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", vente.IdClient);
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference", vente.Reference);
            return View(vente);
        }

        // GET: Ventes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _context.Ventes.FindAsync(id);
            if (vente == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", vente.IdClient);
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference", vente.Reference);
            return View(vente);
        }

        // POST: Ventes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVente,DateVente,Reference,Quantite,IdClient")] Vente vente)
        {
            if (id != vente.IdVente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenteExists(vente.IdVente))
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
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", vente.IdClient);
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference", vente.Reference);
            return View(vente);
        }

        // GET: Ventes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vente = await _context.Ventes
                .Include(v => v.IdClientNavigation)
                .Include(v => v.ReferenceNavigation)
                .FirstOrDefaultAsync(m => m.IdVente == id);
            if (vente == null)
            {
                return NotFound();
            }

            return View(vente);
        }

        // POST: Ventes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vente = await _context.Ventes.FindAsync(id);
            if (vente != null)
            {
                _context.Ventes.Remove(vente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenteExists(int id)
        {
            return _context.Ventes.Any(e => e.IdVente == id);
        }
    }
}
