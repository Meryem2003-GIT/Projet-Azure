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
                                               select prog.Remise).FirstOrDefault()
                                 };

            var factureData = factureDetails.GroupBy(f => new { 
                                                f.FactureId, 
                                                f.FactureTotal, 
                                                f.ClientId, 
                                                f.ClientNom, 
                                                f.ClientPrenom, 
                                                f.ClientEmail })
                                             .Select(group => new FactureModel
                                             {
                                                 FactureId = group.Key.FactureId,
                                                 FactureTotal = group.Key.FactureTotal,
                                                 ClientId = group.Key.ClientId,
                                                 ClientNom = group.Key.ClientNom,
                                                 ClientPrenom = group.Key.ClientPrenom,
                                                 ClientEmail = group.Key.ClientEmail,
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
        public IActionResult AjouterProduit(int? idFacture)
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
                                               select prog.Remise).FirstOrDefault()
                                 };

            if (factureDetails == null)
            {
                return NotFound("Facture introuvable.");
            }

            var factureData = factureDetails.GroupBy(f => new {
                f.FactureId,
                f.FactureTotal,
                f.ClientId,
                f.ClientNom,
                f.ClientPrenom,
                f.ClientEmail
            }).Select(group => new FactureModel
                                             {
                                                 FactureId = group.Key.FactureId,
                                                 FactureTotal = group.Key.FactureTotal,
                                                 ClientId = group.Key.ClientId,
                                                 ClientNom = group.Key.ClientNom,
                                                 ClientPrenom = group.Key.ClientPrenom,
                                                 ClientEmail = group.Key.ClientEmail,
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
        public IActionResult AjouterProduit(int idFacture, int produitId, int quantite)
        {
            var facture = _context.Factures.FirstOrDefault(f => f.IdFacture == idFacture);
            if (facture == null)
            {
                return NotFound("Facture introuvable.");
            }

            var produit = _context.Produits.FirstOrDefault(p => p.Reference == produitId);
            if (produit == null)
            {
                return NotFound("Produit introuvable.");
            }

            // Ajouter le produit à la facture
            var vente = new Vente
            {
                IdFacture = idFacture,
                Reference = produitId,
                Quantite = quantite
            };
            _context.Ventes.Add(vente);
            _context.SaveChanges();

            // Rediriger vers la même page pour afficher la mise à jour
            return RedirectToAction("AjouterProduits", new { idFacture });
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
