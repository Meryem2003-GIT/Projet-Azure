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
    public class PharmaciensController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public PharmaciensController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(LoginModel m)
        {
            if (ModelState.IsValid) {
                var pharmacien = _context.Pharmaciens.Join(_context.Comptes,p => p.Cin, c => c.Cin,(p,c) => new {Pharmacien = p, Compte = c})
                    .FirstOrDefault(pc => pc.Compte.Email == m.Email && pc.Pharmacien.MotPasse == m.Password);
                if (pharmacien != null)
                {
                    return Redirect("/html/accueil.html");
                }
                ViewBag.Message = "Identifiants incorrects.";
            }
            
            
            
            return View(m);
        }


        // GET: Pharmaciens
        public async Task<IActionResult> Index()
        {
            var gestionPharmacieBdContext = _context.Pharmaciens.Include(p => p.CinNavigation);
            return View(await gestionPharmacieBdContext.ToListAsync());
        }

        // GET: Pharmaciens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pharmacien = await _context.Pharmaciens
                .Include(p => p.CinNavigation)
                .FirstOrDefaultAsync(m => m.IdPharmacien == id);
            if (pharmacien == null)
            {
                return NotFound();
            }

            return View(pharmacien);
        }

        // GET: Pharmaciens/Create
        public IActionResult Create()
        {
            ViewData["Cin"] = new SelectList(_context.Comptes, "Cin", "Cin");
            return View();
        }

        // POST: Pharmaciens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cin,IdPharmacien,MotPasse")] Pharmacien pharmacien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pharmacien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cin"] = new SelectList(_context.Comptes, "Cin", "Cin", pharmacien.Cin);
            return View(pharmacien);
        }

        // GET: Pharmaciens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pharmacien = await _context.Pharmaciens.FindAsync(id);
            if (pharmacien == null)
            {
                return NotFound();
            }
            ViewData["Cin"] = new SelectList(_context.Comptes, "Cin", "Cin", pharmacien.Cin);
            return View(pharmacien);
        }

        // POST: Pharmaciens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Cin,IdPharmacien,MotPasse")] Pharmacien pharmacien)
        {
            if (id != pharmacien.IdPharmacien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pharmacien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PharmacienExists(pharmacien.IdPharmacien))
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
            ViewData["Cin"] = new SelectList(_context.Comptes, "Cin", "Cin", pharmacien.Cin);
            return View(pharmacien);
        }

        // GET: Pharmaciens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pharmacien = await _context.Pharmaciens
                .Include(p => p.CinNavigation)
                .FirstOrDefaultAsync(m => m.IdPharmacien == id);
            if (pharmacien == null)
            {
                return NotFound();
            }

            return View(pharmacien);
        }

        // POST: Pharmaciens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pharmacien = await _context.Pharmaciens.FindAsync(id);
            if (pharmacien != null)
            {
                _context.Pharmaciens.Remove(pharmacien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PharmacienExists(int id)
        {
            return _context.Pharmaciens.Any(e => e.IdPharmacien == id);
        }
    }
}
