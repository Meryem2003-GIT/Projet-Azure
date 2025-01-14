using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;
using System.Numerics;
using Humanizer;
using System.Net.Sockets;

namespace gestionPharmacieApp.Controllers
{
    public class ClientsController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public ClientsController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = _context.Clients
                           .Include(c => c.CinNavigation)       
                             .Include(c => c.ProgFidelites)       
                                .ToList();

            // Passer les données à la vue
            return View(clients);
            
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.CinNavigation)
                .FirstOrDefaultAsync(m => m.IdClient == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Compte compte)
        {
            if (ModelState.IsValid)
            {
                _context.Comptes.Add(compte);
                Client c = new Client(); 
                c.Cin = compte.Cin;
                _context.Clients.Add(c);
                _context.SaveChanges();
                var cl = _context.Clients
                .Where(c => c.Cin == compte.Cin)
                .FirstOrDefault();
                ProgFidelite p = new ProgFidelite();
                p.IdClient = cl.IdClient;
                p.Points = 0;
                p.Remise = 0;
                _context.ProgFidelites.Update(p);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var compte = _context.Clients
        .Where(c => c.IdClient == id)
        .Select(c => c.CinNavigation) 
        .FirstOrDefault();

            if (compte == null)
            {
                return NotFound(); 
            }

            

            return View(compte);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Compte compte)
        {
            

            if (ModelState.IsValid)
        {
            try
            {
                _context.Comptes.Update(compte);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirigez vers la liste des comptes
            }
            catch
            {
                return View(compte);
            }
        }

        return View(compte);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var compte = _context.Clients
        .Where(c => c.IdClient == id)
        .Select(c => c.CinNavigation)
        .FirstOrDefault();
            if (compte == null)
            {
                return NotFound();
            }



            return View(compte);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(String? cin)
        {
            var client = await _context.Clients
        .FirstOrDefaultAsync(c => c.Cin == cin);

            var compte = await _context.Comptes
        .FirstOrDefaultAsync(ct => ct.Cin == cin);

            var vente = await _context.Ventes
        .FirstOrDefaultAsync(v => v.IdClient == client.IdClient);

            var pF = await _context.ProgFidelites
       .FirstOrDefaultAsync(p => p.IdClient == client.IdClient);

            if (client == null)
            {
               return NotFound();
                
            }

            _context.ProgFidelites.Remove(pF);
            _context.Ventes.Remove(vente);
            _context.Clients.Remove(client);
            _context.Comptes.Remove(compte);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.IdClient == id);
        }

        // GET: Produits/Search
        public async Task<IActionResult> Search(string searchType, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return View("Index", await _context.Clients.ToListAsync());
            }

            IEnumerable<Client> clients;

            if (searchType == "cin")
            {
                
                clients = await _context.Clients
                           .Include(c => c.CinNavigation)       
                             .Include(c => c.ProgFidelites)       
                                .Where(c => c.CinNavigation.Cin.Contains(keyword))
                    .ToListAsync();
            }
            else if (searchType == "nom" )
            {
                clients = await _context.Clients
                          .Include(c => c.CinNavigation)
                            .Include(c => c.ProgFidelites)
                               .Where( c => c.CinNavigation.Nom.Contains(keyword))
                   .ToListAsync();
            }
            else
            {
                clients = new List<Client>();
            }

            return View("Index", clients);
        }
    }
}
