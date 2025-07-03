using Microsoft.AspNetCore.Mvc;
using appWeb.Models;
using appWeb.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace appWeb.Controllers
{
    public class EncadrantController : Controller
    {
        private readonly ApplicationDbContext _context;
       

        public EncadrantController(ApplicationDbContext context )
        {
            
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5; // Nombre d'encadrants par page

           

            var query = _context.Encadrants
                .Include(e => e.Formation)
                .OrderBy(e => e.Nom);
            
            int totalEncadrants = await query.CountAsync();

            var encadrants = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEncadrants / pageSize);

            return View(encadrants);
        }


        // GET: Encadrants/Create
        public IActionResult Create()
        {
            // Charger les formations non encore associées à un encadrant
            

            var formationsDisponibles = _context.Formations
                .Include(f => f.Encadrant)
                .Where(f => f.Encadrant == null)
                .ToList();

            ViewBag.Formations = new SelectList(formationsDisponibles, "Id", "Titre");

            return View();
        }

        // POST: Encadrants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Encadrant encadrant)
        {
            

            if (!ModelState.IsValid)
            {
                
                _context.Add(encadrant);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // En cas d'erreur, recharger la liste des formations disponibles
            var formationsDisponibles = _context.Formations
                .Include(f => f.Encadrant)
                .Where(f => f.Encadrant == null || f.Id == encadrant.FormationId)
                .ToList();

            ViewBag.Formations = new SelectList(formationsDisponibles, "Id", "Titre", encadrant.FormationId);

            return View(encadrant);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var encadrant = await _context.Encadrants
                .Include(e => e.Formation)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encadrant == null)
                return NotFound();

            ViewBag.Formations = new SelectList(await _context.Formations.ToListAsync(), "Id", "Titre", encadrant.FormationId);

            return View(encadrant);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Encadrant encadrant)
        {
            if (id != encadrant.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(encadrant);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Encadrants.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.Formations = new SelectList(await _context.Formations.ToListAsync(), "Id", "Titre", encadrant.FormationId);
            return View(encadrant);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var encadrant = await _context.Encadrants
                .Include(e => e.Formation)
                .FirstOrDefaultAsync(e => e.Id == id);

            return encadrant == null ? NotFound() : View(encadrant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var encadrant = await _context.Encadrants.FindAsync(id);
            if (encadrant != null)
            {
                _context.Encadrants.Remove(encadrant);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            var encadrant = await _context.Encadrants
                .Include(e => e.Formation)
                .FirstOrDefaultAsync(e => e.Id == id);

            return encadrant == null ? NotFound() : View(encadrant);
        }

    }
}
