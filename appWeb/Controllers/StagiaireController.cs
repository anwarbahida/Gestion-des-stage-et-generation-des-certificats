using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appWeb.Data;
using appWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace appWeb.Controllers
{
    [Authorize]
    public class StagiaireController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<Admin> _userManager;

        public StagiaireController(ApplicationDbContext context, UserManager<Admin> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var admin = await _userManager.GetUserAsync(User);

            int pageSize = 5;
            var totalItems = await _context.Stagiaires.CountAsync();

            var stagiaires = await _context.Stagiaires
                .OrderBy(s => s.Nom)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewData["SearchType"] = "stagiaire";

            return View(stagiaires);
        }

        // GET: Stagiaire/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var stagiaire = await _context.Stagiaires.FirstOrDefaultAsync(m => m.Id == id);
            if (stagiaire == null) return NotFound();

            return View(stagiaire);
        }

        // GET: Stagiaire/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stagiaire/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom,Prenom,Email,DateNaissance")] Stagiaire stagiaire)
        {
            if (ModelState.IsValid)
            {
    
                _context.Add(stagiaire);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stagiaire);
        }

        // GET: Stagiaire/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var stagiaire = await _context.Stagiaires.FindAsync(id);
            if (stagiaire == null) return NotFound();

            return View(stagiaire);
        }

        // POST: Stagiaire/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prenom,Email,DateNaissance")] Stagiaire stagiaire)
        {
            if (id != stagiaire.Id) return NotFound();

            if (ModelState.IsValid)
            {
                
                try
                {
                    _context.Update(stagiaire);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StagiaireExists(stagiaire.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(stagiaire);
        }

        // GET: Stagiaire/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var stagiaire = await _context.Stagiaires.FirstOrDefaultAsync(m => m.Id == id);
            if (stagiaire == null) return NotFound();

            return View(stagiaire);
        }

        // POST: Stagiaire/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stagiaire = await _context.Stagiaires.FindAsync(id);
            if (stagiaire != null)
            {
                _context.Stagiaires.Remove(stagiaire);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StagiaireExists(int id)
        {
            return _context.Stagiaires.Any(e => e.Id == id);
        }
    }
}
