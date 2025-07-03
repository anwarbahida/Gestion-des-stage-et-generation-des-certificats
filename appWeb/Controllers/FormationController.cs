using Microsoft.AspNetCore.Mvc;           // contexte EF (DbContext)
using System.Threading.Tasks;
using System.Linq;
using appWeb.Data;
using appWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace appWab.Controllers
{
    [Authorize]
    public class FormationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Formation

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            var totalItems = await Task.Run(() => _context.Formations.Count());

            var formations = await Task.Run(() =>
                _context.Formations
                    .OrderBy(f => f.DateDebut)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
            );

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(formations);
        }
       /* public async Task<IActionResult> Index()
        {
            var formations = await Task.Run(() => _context.Formations.ToList());
            return View(formations);
        }*/

        // GET: Formation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var formation = await Task.Run(() => _context.Formations.FirstOrDefault(f => f.Id == id));
            if (formation == null)
                return NotFound();

            return View(formation);
        }

        // GET: Formation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Formation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Formation formation)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(formation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(formation);
        }

        // GET: Formation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var formation = await Task.Run(() => _context.Formations.Find(id));
            if (formation == null)
                return NotFound();

            return View(formation);
        }

        // POST: Formation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Formation formation)
        {
            if (id != formation.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(formation);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    if (!_context.Formations.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(formation);
        }

        // GET: Formation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var formation = await Task.Run(() => _context.Formations.FirstOrDefault(f => f.Id == id));
            if (formation == null)
                return NotFound();

            return View(formation);
        }

        // POST: Formation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formation = await Task.Run(() => _context.Formations.Find(id));
            if (formation != null)
            {
                _context.Formations.Remove(formation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
