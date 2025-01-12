using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;

namespace gestionPharmacieApp.Controllers
{
    public class FournisseursController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public FournisseursController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: Fournisseurs
        public async Task<IActionResult> Index(string searchType, string keyword)
        {
            var fournisseurs = _context.Fournisseurs.AsQueryable();

            // Gestion de la recherche
            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(keyword))
            {
                switch (searchType)
                {
                    case "IdFournisseur":
                        if (int.TryParse(keyword, out int idFournisseur))
                        {
                            fournisseurs = fournisseurs.Where(f => f.IdFournisseur == idFournisseur);
                        }
                        break;
                    case "NomSociete":
                        fournisseurs = fournisseurs.Where(f => f.NomSociete.Contains(keyword));
                        break;
                    case "Email":
                        fournisseurs = fournisseurs.Where(f => f.Email.Contains(keyword));
                        break;
                }
            }

            return View(await fournisseurs.ToListAsync());
        }

        // GET: Fournisseurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fournisseur = await _context.Fournisseurs
                .FirstOrDefaultAsync(m => m.IdFournisseur == id);
            if (fournisseur == null)
            {
                return NotFound();
            }

            return View(fournisseur);
        }

        // GET: Fournisseurs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fournisseurs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdFournisseur,NomSociete,Adresse,Email")] Fournisseur fournisseur)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fournisseur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fournisseur);
        }


        // GET: Fournisseurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur == null)
            {
                return NotFound();
            }
            return View(fournisseur);
        }

        // POST: Fournisseurs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFournisseur,NomSociete,Adresse,Email")] Fournisseur fournisseur)
        {
            if (id != fournisseur.IdFournisseur)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fournisseur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FournisseurExists(fournisseur.IdFournisseur))
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
            return View(fournisseur);
        }

        // GET: Fournisseurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fournisseur = await _context.Fournisseurs
                .FirstOrDefaultAsync(m => m.IdFournisseur == id);
            if (fournisseur == null)
            {
                return NotFound();
            }

            return View(fournisseur);
        }

        // POST: Fournisseurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur != null)
            {
                _context.Fournisseurs.Remove(fournisseur);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FournisseurExists(int id)
        {
            return _context.Fournisseurs.Any(e => e.IdFournisseur == id);
        }
    }
}
    