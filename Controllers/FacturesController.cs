using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;
using System.Collections;

namespace gestionPharmacieApp.Controllers
{
    public class FacturesController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public FacturesController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> factureClient(Facture f)
        {
            var factureDetails = from facture in _context.Factures
                                 join vente in _context.Ventes on facture.IdFacture equals vente.IdFacture
                                 join produit in _context.Produits on vente.Reference equals produit.Reference
                                 join client in _context.Clients on vente.IdClient equals client.IdClient
                                 join compte in _context.Comptes on client.Cin equals compte.Cin
                                 where facture.IdFacture == f.IdFacture
                                 select new
                                 {
                                     FactureId = facture.IdFacture,
                                     FactureTotal = facture.Total,
                                     ClientNom = compte.Nom,
                                     ClientId = client.IdClient,
                                     ClientPrenom = compte.Prenom,
                                     ClientEmail = compte.Email,
                                     ProduitLibelle = produit.Libelle,
                                     ProduitPrix = produit.Prix,
                                     Quantite = vente.Quantite,
                                     MontantProduit = produit.Prix * vente.Quantite,
                                     Remise = (from prog in _context.ProgFidelites
                                               where prog.IdClient == client.IdClient
                                               select prog.Remise).FirstOrDefault(),
                                    DateFacture = vente.DateVente
                                 };

            var factureData = factureDetails.GroupBy(f => new { 
                                                f.FactureId, 
                                                f.FactureTotal, 
                                                f.ClientId, 
                                                f.ClientNom, 
                                                f.ClientPrenom, 
                                                f.ClientEmail,
                                                f.Remise,
                                                f.DateFacture
                                                })
                                             .Select(group => new FactureModel
                                             {
                                                 FactureId = group.Key.FactureId,
                                                 FactureTotal = group.Key.FactureTotal,
                                                 ClientId = group.Key.ClientId,
                                                 ClientNom = group.Key.ClientNom,
                                                 ClientPrenom = group.Key.ClientPrenom,
                                                 ClientEmail = group.Key.ClientEmail,
                                                 Remise = (double)group.Key.Remise,
                                                 dateFacture = group.Key.DateFacture,
                                                 Produits = group.Select(p => new ProduitDetailsModel
                                                 {
                                                     ProduitLibelle = p.ProduitLibelle,
                                                     ProduitPrix = p.ProduitPrix,
                                                     Quantite = p.Quantite,
                                                     MontantProduit = p.MontantProduit,
                                                     
                                                 }).ToList()
                                             }).FirstOrDefault();

            if (factureData == null)
            {
                return NotFound();
            }

            return View(factureData);
        }


        [HttpGet]
        public IActionResult AjouterProduit(int? idFacture, int idClient)
        {
            var factureDetails = from facture in _context.Factures
                                 join vente in _context.Ventes on facture.IdFacture equals vente.IdFacture
                                 join produit in _context.Produits on vente.Reference equals produit.Reference
                                 join client in _context.Clients on vente.IdClient equals client.IdClient
                                 join compte in _context.Comptes on client.Cin equals compte.Cin
                                 where facture.IdFacture == idFacture
                                 select new
                                 {
                                     FactureId = facture.IdFacture,
                                     FactureTotal = facture.Total,
                                     ClientNom = compte.Nom,
                                     ClientId = client.IdClient,
                                     ClientPrenom = compte.Prenom,
                                     ClientEmail = compte.Email,
                                     ProduitLibelle = produit.Libelle,
                                     ProduitPrix = produit.Prix,
                                     Quantite = vente.Quantite,
                                     MontantProduit = produit.Prix * vente.Quantite,
                                     Remise = (from prog in _context.ProgFidelites
                                               where prog.IdClient == client.IdClient
                                               select prog.Remise).FirstOrDefault(),
                                     DateFacture = vente.DateVente
                                 };

            var factureData = factureDetails.GroupBy(f => new {
                f.FactureId,
                f.FactureTotal,
                f.ClientId,
                f.ClientNom,
                f.ClientPrenom,
                f.ClientEmail,
                f.Remise,
                f.DateFacture
            })
                                             .Select(group => new FactureModel
                                             {
                                                 FactureId = group.Key.FactureId,
                                                 FactureTotal = group.Key.FactureTotal,
                                                 ClientId = group.Key.ClientId,
                                                 ClientNom = group.Key.ClientNom,
                                                 ClientPrenom = group.Key.ClientPrenom,
                                                 ClientEmail = group.Key.ClientEmail,
                                                 Remise = (double)group.Key.Remise,
                                                 dateFacture = group.Key.DateFacture,
                                                 Produits = group.Select(p => new ProduitDetailsModel
                                                 {
                                                     ProduitLibelle = p.ProduitLibelle,
                                                     ProduitPrix = p.ProduitPrix,
                                                     Quantite = p.Quantite,
                                                     MontantProduit = p.MontantProduit,

                                                 }).ToList()
                                             }).FirstOrDefault();
            factureData.ProduitsBD = _context.Produits.Select(p => new SelectListItem
            {
                Value = p.Reference.ToString(),
                Text = p.Libelle
            }).ToList();

            if (factureData == null)
            {
                return NotFound();
            }

            return View(factureData);
        }

        [HttpPost]
        public async Task<IActionResult> AjouterProduit(int idFacture, int ReferenceProduit, int Quantite, int IdClient)
        {
            if (ReferenceProduit == null || Quantite <= 0)
            {
                // Gestion des erreurs (ex: produit non sélectionné ou quantité invalide)
                
                TempData["ErrorMessage"] = "Produit ou quantité invalide.";
                return RedirectToAction("AjouterProduit", new { idFacture = idFacture });
            }
            var facture =  _context.Factures.FirstOrDefault(f => f.IdFacture == idFacture);
            if (facture == null)
            {
                return NotFound("Facture introuvable.");
            }

            var produit = _context.Produits.FirstOrDefault(p => p.Reference == ReferenceProduit);
            if (produit == null)
            {
                return NotFound("Produit introuvable.");
            }

            // Ajouter le produit à la facture
            var vente = new Vente
            {
                IdFacture = idFacture,
                Reference = ReferenceProduit,
                Quantite = Quantite,
                IdClient = IdClient
            };
            _context.Ventes.Add(vente);
            _context.SaveChanges();
            var programmeFidelite = await _context.ProgFidelites.FirstOrDefaultAsync(p => p.IdClient == IdClient);
            if (programmeFidelite != null)
            {
                double totalVente = produit.Prix * vente.Quantite;
                programmeFidelite.Points += (int?)(programmeFidelite.Points + totalVente);
                if (programmeFidelite.Points >= 1000)
                {
                    programmeFidelite.Remise = 2.0;
                    // Met à jour la facture avec le total

                    facture.Total += totalVente - (totalVente * 0.02);
                    programmeFidelite.Points -= 1000;
                    _context.Factures.Update(facture);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    programmeFidelite.Remise = 0.0;
                    facture.Total =  facture.Total+totalVente;
                    _context.Factures.Update(facture);
                    await _context.SaveChangesAsync();
                }
                _context.ProgFidelites.Update(programmeFidelite);
                await _context.SaveChangesAsync();
            }
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Reference == vente.Reference);
            if (stock == null || stock.Quantite < vente.Quantite)
            {
                ModelState.AddModelError("", $"le stock est{stock.Quantite} la quantite est : {vente.Quantite}");
                ModelState.AddModelError("", "Quantité demandée non disponible ou stock introuvable.");
                
                return View(vente);
            }

            // Met à jour le stock
            stock.Quantite -= vente.Quantite;
            _context.Stocks.Update(stock);
            
            // Rediriger vers la même page pour afficher la mise à jour
            return RedirectToAction("factureClient", "Factures", facture);
        }
        public async Task<IActionResult> Index()
        {
            var factures = from f in _context.Factures
                           join v in _context.Ventes on f.IdFacture equals v.IdFacture
                           join p in _context.Produits on v.Reference equals p.Reference
                           join c in _context.Clients on v.IdClient equals c.IdClient
                           join ct in _context.Comptes on c.Cin equals ct.Cin
                           where v.DateVente >= new DateOnly(2025, 01, 01) && v.DateVente <= new DateOnly(2025, 12, 31)
                           select new
                           {
                               FactureId = f.IdFacture,
                               TotalFacture = f.Total,
                               DateVente = v.DateVente,
                               Quantite = v.Quantite,
                               ProduitLibelle = p.Libelle,
                               ProduitPrix = p.Prix,
                               ClientNom = ct.Prenom + " " + ct.Nom,
                               ClientEmail = ct.Email
                           };

            var resultList = await factures.ToListAsync();
            return View(resultList);

        }


        // POST: Factures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facture = await _context.Factures.FindAsync(id);
            if (facture != null)
            {
                _context.Factures.Remove(facture);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FactureExists(int id)
        {
            return _context.Factures.Any(e => e.IdFacture == id);
        }
    }
}
