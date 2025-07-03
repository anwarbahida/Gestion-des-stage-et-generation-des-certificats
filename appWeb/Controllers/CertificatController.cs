using appWeb.Data;
using appWeb.Models;
using appWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
namespace appWeb.Controllers
{
    [Authorize]
    public class CertificatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CertificatService _certificatService;
        private readonly UserManager<Admin> _userManager;

        public CertificatController(ApplicationDbContext context, CertificatService certificatService, UserManager<Admin> userManager)
        {
            _context = context;
            _certificatService = certificatService;
            _userManager = userManager;
        }

        // GET: Certificat
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            var totalItems = await _context.Certificats.CountAsync();

            var certificats = await _context.Certificats
                .Include(c => c.Stagiaire)
                .Include(c => c.Formation)
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(certificats);
        }
 

        // GET: Certificat/Create
        public IActionResult Create()
        {
            ViewData["StagiaireId"] = new SelectList(
                _context.Stagiaires
                    .Select(s => new {
                        Id = s.Id,
                        NomComplet = s.Nom + " " + s.Prenom
                    }),
                "Id",
                "NomComplet"
            );

            ViewData["FormationId"] = new SelectList(_context.Formations, "Id", "Titre");

            return View();
        }

        // POST: Certificat/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StagiaireId,FormationId,DateGeneration")] Certificat certificat)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(certificat);
                await _context.SaveChangesAsync();

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // ou log dans un fichier ou Debug.WriteLine
                }

                return RedirectToAction(nameof(Index));
            }

            // Vérification de l'existence du certificat

            ChargerViewBags(certificat);
            Console.WriteLine($" StagiaireId: {certificat.StagiaireId}, FormationId: {certificat.FormationId}, Date: {certificat.DateGeneration}");

            return View(certificat);
        }

        // 🔽 Méthode privée pour éviter la répétition du code de ViewData
        private void ChargerViewBags(Certificat certificat)
        {
            ViewData["StagiaireId"] = new SelectList(_context.Stagiaires, "Id", "Nom", certificat.StagiaireId);
            ViewData["FormationId"] = new SelectList(_context.Formations, "Id", "Titre", certificat.FormationId);
        }

        // GET: Certificat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var certificat = await _context.Certificats.FindAsync(id);
            if (certificat == null) return NotFound();

            ViewData["StagiaireId"] = new SelectList(
                _context.Stagiaires.Select(s => new {
                    Id = s.Id,
                    NomComplet = s.Nom + " " + s.Prenom
                }),
                "Id",
                "NomComplet",
                certificat.StagiaireId
            );
            ViewData["FormationId"] = new SelectList(_context.Formations, "Id", "Titre", certificat.FormationId);
            return View(certificat);
        }

        // POST: Certificat/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StagiaireId,FormationId,DateGeneration")] Certificat certificat)
        {
            if (id != certificat.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                    _context.Update(certificat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificatExists(certificat.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["StagiaireId"] = new SelectList(
                _context.Stagiaires.Select(s => new {
                    Id = s.Id,
                    NomComplet = s.Nom + " " + s.Prenom
                }),
                "Id",
                "NomComplet",
                certificat.StagiaireId
            );
            ViewData["FormationId"] = new SelectList(_context.Formations, "Id", "Titre", certificat.FormationId);
            return View(certificat);
        }

        // GET: Certificat/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var certificat = await _context.Certificats
                .Include(c => c.Stagiaire)
                .Include(c => c.Formation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (certificat == null) return NotFound();

            return View(certificat);
        }

        // POST: Certificat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificat = await _context.Certificats.FindAsync(id);
            if (certificat != null)
            {
                _context.Certificats.Remove(certificat);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Certificat/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var certificat = await _context.Certificats
                .Include(c => c.Stagiaire)
                .Include(c => c.Formation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (certificat == null) return NotFound();

            return View(certificat);
        }

        // Télécharger le PDF
        [Obsolete]
        public async Task<IActionResult> TelechargerPdf(int id)
        {
            var certificat = await _context.Certificats
                .Include(c => c.Stagiaire)
                .Include(c => c.Formation)
                    .ThenInclude(f => f.Encadrant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (certificat == null)
                return NotFound();

            var admin = await _userManager.GetUserAsync(User);
            var pdfBytes = _certificatService.GenererCertificat(certificat.Stagiaire, certificat.Formation, certificat.Formation.Encadrant, admin);
            return File(pdfBytes, "application/pdf", $"Certificat_{certificat.Stagiaire.Nom}_{certificat.Formation.Titre}.pdf");
        }


        private bool CertificatExists(int id)
        {
            return _context.Certificats.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Generer(int page = 1)
        {
            int pageSize = 6;
            var totalItems = await _context.Certificats.CountAsync();

            var certificats = await _context.Certificats
                .Include(c => c.Stagiaire)
                .Include(c => c.Formation)
                    .ThenInclude(f => f.Encadrant) // 👈 AJOUT IMPORTANT
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewData["SearchType"] = "Certificats";

            return View(certificats);
        }
        [Obsolete]
        public async Task<IActionResult> ImprimerPdf(int id)
        {
            var certificat = await _context.Certificats
                .Include(c => c.Stagiaire)
                .Include(c => c.Formation)
                    .ThenInclude(f => f.Encadrant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (certificat == null)
                return NotFound();
            var admin = await _userManager.GetUserAsync(User);
            var pdfBytes = _certificatService.GenererCertificat(
                certificat.Stagiaire,
                certificat.Formation,
                certificat.Formation.Encadrant,
                admin);

            var nomFichier = $"Certificat_{certificat.Stagiaire.Nom}_{certificat.Formation.Titre}.pdf";
            var nomFichierEncode = Uri.EscapeDataString(nomFichier);

            Response.Headers.Append("Content-Disposition", $"inline; filename*=UTF-8''{nomFichierEncode}");

     
            return File(pdfBytes, "application/pdf");
        }
    }
}
