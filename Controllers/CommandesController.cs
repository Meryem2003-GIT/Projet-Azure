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
    public class CommandesController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public CommandesController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: Commandes
        public async Task<IActionResult> Index()
        {
            var gestionPharmacieBdContext = _context.Commandes.Include(c => c.IdFournisseurNavigation).Include(c => c.ReferenceNavigation);
            return View(await gestionPharmacieBdContext.ToListAsync());
        }

        // GET: Commandes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commande = await _context.Commandes
                .Include(c => c.IdFournisseurNavigation)
                .Include(c => c.ReferenceNavigation)
                .FirstOrDefaultAsync(m => m.IdCommande == id);
            if (commande == null)
            {
                return NotFound();
            }

            return View(commande);
        }

        // GET: Commandes/Create
        public IActionResult Create()
        {
            ViewData["IdFournisseur"] = new SelectList(_context.Fournisseurs, "IdFournisseur", "IdFournisseur");
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference");
            return View();
        }

        // POST: Commandes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCommande,DateCommande,Reference,Quantite,IdFournisseur")] Commande commande)
        {
            if (ModelState.IsValid)
            {
                _context.Add(commande);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdFournisseur"] = new SelectList(_context.Fournisseurs, "IdFournisseur", "IdFournisseur", commande.IdFournisseur);
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference", commande.Reference);
            return View(commande);
        }

        // GET: Commandes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
            {
                return NotFound();
            }
            ViewData["IdFournisseur"] = new SelectList(_context.Fournisseurs, "IdFournisseur", "IdFournisseur", commande.IdFournisseur);
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference", commande.Reference);
            return View(commande);
        }

        // POST: Commandes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCommande,DateCommande,Reference,Quantite,IdFournisseur")] Commande commande)
        {
            if (id != commande.IdCommande)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(commande);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommandeExists(commande.IdCommande))
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
            ViewData["IdFournisseur"] = new SelectList(_context.Fournisseurs, "IdFournisseur", "IdFournisseur", commande.IdFournisseur);
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Reference", commande.Reference);
            return View(commande);
        }

        // GET: Commandes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commande = await _context.Commandes
                .Include(c => c.IdFournisseurNavigation)
                .Include(c => c.ReferenceNavigation)
                .FirstOrDefaultAsync(m => m.IdCommande == id);
            if (commande == null)
            {
                return NotFound();
            }

            return View(commande);
        }

        // POST: Commandes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande != null)
            {
                _context.Commandes.Remove(commande);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommandeExists(int id)
        {
            return _context.Commandes.Any(e => e.IdCommande == id);
        }
    }
}
