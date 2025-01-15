using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

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
            var gestionPharmacieBdContext = _context.Ventes.Include(v => v.IdClientNavigation).Include(v => v.ReferenceNavigation)
                .Include(f => f.IdFactureNavigation);
            return View(await gestionPharmacieBdContext.ToListAsync());
        }

        // GET: Ventes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound(); 
            }
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
        // GET: Produits/Search
        public async Task<IActionResult> Search(string searchType, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return View("Index", await _context.Ventes.ToListAsync());
            }

            IEnumerable<Vente> ventes;

            if (searchType == "cin")
            {
                ventes = await _context.Ventes
                    .Include(v => v.IdClientNavigation)
                    .Include(v => v.ReferenceNavigation)
                    .Where(v => v.IdClientNavigation.Cin.Contains(keyword))
                    .ToListAsync();
            }
            else if (searchType == "libelle" )
            {
                ventes = await _context.Ventes
                    .Where(v => v.ReferenceNavigation.Libelle.Contains(keyword))
                    .ToListAsync();
            }
            else
            {
                ventes = new List<Vente>();
            }

            return View("Index", ventes);
        }

        // GET: Ventes/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound("ID client non spécifié.");
            }
            var vente = new Vente
            {
                IdClient = id.Value
            };
            
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Libelle");
            return View(vente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVente,DateVente,Reference,Quantite,IdClient")] Vente vente)
        {
            {
                Vente vt = new Vente()
                {
                    IdClient = vente.IdClient,
                    DateVente = DateOnly.FromDateTime(DateTime.Now),
                    Reference = vente.Reference,
                    Quantite = vente.Quantite,
                };
                try
                {
                    // Définit la date de vente
                    vente.DateVente = DateOnly.FromDateTime(DateTime.Now);

                    // Vérifie si le produit existe
                    var produit = await _context.Produits.FirstOrDefaultAsync(p => p.Reference == vente.Reference);
                    if (produit == null)
                    {
                        ModelState.AddModelError("", "Le produit spécifié n'existe pas.");
                        ResetViewData(vt);
                        return View(vente);
                    }

                    // Vérifie si le stock est suffisant
                    var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Reference == vente.Reference);
                    if (stock == null || stock.Quantite < vente.Quantite )
                    {
                        ModelState.AddModelError("", $"le stock est{stock.Quantite} la quantite est : {vente.Quantite}");
                        ModelState.AddModelError("", "Quantité demandée non disponible ou stock introuvable.");
                        ResetViewData(vt);
                        return View(vente);
                    }

                    // Crée une nouvelle facture
                    var facture = new Facture { Total = 0 };
                    _context.Factures.Add(facture);
                    await _context.SaveChangesAsync();

                    // Associe la facture à la vente
                    vente.IdFacture = facture.IdFacture;

                    // Ajoute la vente
                    _context.Ventes.Add(vente);
                    await _context.SaveChangesAsync();

                    // Met à jour les points de fidélité
                    var programmeFidelite = await _context.ProgFidelites.FirstOrDefaultAsync(p => p.IdClient == vente.IdClient);
                    if (programmeFidelite != null)
                    {
                        double totalVente = produit.Prix * vente.Quantite;
                        programmeFidelite.Points += (int)Math.Floor(totalVente);
                        programmeFidelite.Remise = 0.0;
                        if (programmeFidelite.Points >= 1000)
                        {
                            programmeFidelite.Remise = 2.0;
                            // Met à jour la facture avec le total

                            facture.Total += totalVente - (totalVente * 0.02);
                            programmeFidelite.Points -= 100;
                            
                            
                        }
                        else {
                            programmeFidelite.Remise = 0.0;
                            facture.Total = facture.Total + totalVente;
                        }
                        _context.ProgFidelites.Update(programmeFidelite);
                        await _context.SaveChangesAsync();
                    }

                    // Met à jour le stock
                    stock.Quantite -= vente.Quantite;
                    _context.Stocks.Update(stock);

                    // Enregistre les modifications
                    await _context.SaveChangesAsync();
                    return RedirectToAction("factureClient", "Factures",facture);
                }
                catch (Exception ex)
                {
                    // En cas d'exception, nettoyez le suivi du DbContext
                    _context.ChangeTracker.Clear();
                    ModelState.AddModelError("", $"Une erreur inattendue est{vente.IdClient} survenue : {ex.InnerException?.Message ?? ex.Message}");
                    ResetViewData(vt);
                    return View(vente);
                }
            }
        }

// Méthode auxiliaire pour réinitialiser les listes déroulantes
private void ResetViewData(Vente vente)
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "IdClient", vente.IdClient);
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Libelle", vente.Reference);
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
