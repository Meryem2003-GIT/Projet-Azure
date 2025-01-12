using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gestionPharmacieApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gestionPharmacieApp.Controllers
{
    public class StocksController : Controller
    {
        private readonly GestionPharmacieBdContext _context;

        public StocksController(GestionPharmacieBdContext context)
        {
            _context = context;
        }

        // GET: Stocks
        public async Task<IActionResult> Index(string searchType, string keyword)
        {
            var stocks = _context.Stocks.Include(s => s.ReferenceNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(keyword))
            {
                switch (searchType)
                {
                    case "IdStock":
                        if (int.TryParse(keyword, out int id))
                            stocks = stocks.Where(s => s.IdStock == id);
                        break;
                    case "Reference":
                        if (int.TryParse(keyword, out int reference))
                            stocks = stocks.Where(s => s.Reference == reference);
                        break;
                }
            }

            return View(await stocks.ToListAsync());
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Libelle");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStock,Reference,Quantite")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Libelle", stock.Reference);
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return NotFound();

            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Libelle", stock.Reference);
            return View(stock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStock,Reference,Quantite")] Stock stock)
        {
            if (id != stock.IdStock) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Stocks.Any(s => s.IdStock == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Reference"] = new SelectList(_context.Produits, "Reference", "Libelle", stock.Reference);
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks.Include(s => s.ReferenceNavigation).FirstOrDefaultAsync(m => m.IdStock == id);
            return stock == null ? NotFound() : View(stock);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null) _context.Stocks.Remove(stock);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
