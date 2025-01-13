using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;

namespace gestionPharmacieApp.Controllers
{
    public class ComptesController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public ComptesController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: Comptes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comptes.ToListAsync());
        }

        // GET: Comptes/Details/{Cin}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var compte = await _context.Comptes
                .FirstOrDefaultAsync(m => m.Cin == id);
            if (compte == null)
            {
                return NotFound();
            }

            return View(compte);
        }

        // GET: Comptes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comptes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cin,Prenom,Nom,Adresse,DateNaissance,Email,Telephone")] Compte compte)
        {
            if (ModelState.IsValid)
            {
                _context.Add(compte);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(compte);
        }

        // GET: Comptes/Edit/{Cin}
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
            {
                return NotFound();
            }
            return View(compte);
        }

        // POST: Comptes/Edit/{Cin}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Cin,Prenom,Nom,Adresse,DateNaissance,Email,Telephone")] Compte compte)
        {
            if (id != compte.Cin)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compte);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompteExists(compte.Cin))
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
            return View(compte);
        }

        // GET: Comptes/Delete/{Cin}
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var compte = await _context.Comptes
                .FirstOrDefaultAsync(m => m.Cin == id);
            if (compte == null)
            {
                return NotFound();
            }

            return View(compte);
        }

        // POST: Comptes/Delete/{Cin}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte != null)
            {
                _context.Comptes.Remove(compte);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompteExists(string id)
        {
            return _context.Comptes.Any(e => e.Cin == id);
        }
    }
}
