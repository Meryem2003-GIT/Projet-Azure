using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gestionPharmacieApp.Controllers
{
    public class ProduitsController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public ProduitsController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: Produits
        public async Task<IActionResult> Index()
        {
            return View(await _context.Produits.ToListAsync());
        }

        // GET: Produits/Search
        public async Task<IActionResult> Search(string searchType, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return View("Index", await _context.Produits.ToListAsync());
            }

            IEnumerable<Produit> produits;

            if (searchType == "Libelle")
            {
                produits = await _context.Produits
                    .Where(p => p.Libelle.Contains(keyword))
                    .ToListAsync();
            }
            else if (searchType == "Reference" && int.TryParse(keyword, out int reference))
            {
                produits = await _context.Produits
                    .Where(p => p.Reference == reference)
                    .ToListAsync();
            }
            else
            {
                produits = new List<Produit>();
            }

            return View("Index", produits);
        }

        // GET: Produits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Reference,DescriptionP,Libelle,Prix")] Produit produit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produit);
        }

        // GET: Produits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
            {
                return NotFound();
            }
            return View(produit);
        }

        // POST: Produits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Reference,DescriptionP,Libelle,Prix")] Produit produit)
        {
            if (id != produit.Reference)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduitExists(produit.Reference))
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
            return View(produit);
        }

        // GET: Produits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits
                .FirstOrDefaultAsync(m => m.Reference == id);
            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // POST: Produits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit != null)
            {
                _context.Produits.Remove(produit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProduitExists(int id)
        {
            return _context.Produits.Any(e => e.Reference == id);
        }

        // GET: Produits/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits
                .FirstOrDefaultAsync(m => m.Reference == id);

            if (produit == null)
            {
                return NotFound();
            }

            // Passe la liste complète des produits à la vue
            var produits = await _context.Produits.ToListAsync();

            // Crée un ViewModel pour afficher la liste et les détails d'un produit spécifique
            var viewModel = new ProduitDetailsViewModel
            {
                ProduitSelectionne = produit,
                ListeProduits = produits
            };

            return View("DetailsListe", viewModel); // Utilise une nouvelle vue "DetailsListe"
        }

        public IActionResult CommanderProduit(int id)
        {
            var produit = _context.Produits.FirstOrDefault(p => p.Reference == id);

            if (produit == null)
            {
                return NotFound();
            }

            var commande = new Commande
            {
                Reference = produit.Reference,
                DateCommande = DateOnly.FromDateTime(DateTime.Now)
            };

            ViewBag.Fournisseurs = new SelectList(_context.Fournisseurs, "IdFournisseur", "NomSociete");

            return View(commande);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommanderProduit(Commande commande)
        {
            if (ModelState.IsValid)
            {
                _context.Commandes.Add(commande);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Fournisseurs = new SelectList(_context.Fournisseurs, "IdFournisseur", "NomSociete");
            return View(commande);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprouverCommande(int reference, int quantite, DateTime dateCommande, int idFournisseur)
        {
            if (quantite <= 0 || idFournisseur == 0)
            {
                TempData["Error"] = "Les informations de la commande sont invalides.";
                return RedirectToAction("Details", new { id = reference });
            }

            // Convertir DateTime en DateOnly
            var dateCommandeOnly = DateOnly.FromDateTime(dateCommande);

            // Ajouter la commande à la table Commandes
            var commande = new Commande
            {
                Reference = reference,
                Quantite = quantite,
                DateCommande = dateCommandeOnly,
                IdFournisseur = idFournisseur
            };
            _context.Commandes.Add(commande);

            // Vérifier si le produit existe déjà dans le stock
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Reference == reference);
            if (stock == null)
            {
                stock = new Stock
                {
                    Reference = reference,
                    Quantite = quantite
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                stock.Quantite += quantite;
                _context.Stocks.Update(stock);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Commande approuvée et stock mis à jour.";
            return RedirectToAction("Details", new { id = reference });
        }



    }
}
