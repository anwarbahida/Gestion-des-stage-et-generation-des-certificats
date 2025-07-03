using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appWeb.Data;
using System.Linq;

namespace appWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1 , int pageFormation = 1)
        {
            int pageSize = 3; // Nombre d'éléments par page
            int pageSizeFormation = 4; 


            // 🧮 Compteurs principaux
            var stagiaireCount = _context.Stagiaires.Count();
            var formationCount = _context.Formations.Count();
            var certificatCount = _context.Certificats.Count();
            var EncadrantCount = _context.Encadrants.Count();

            // ✅ Pagination stagiaires
            var query = _context.Certificats
                .Include(c => c.Stagiaire)
                .Include(c => c.Formation)
                .OrderByDescending(c => c.Id);

            var totalStagiaires = query.Count();

            var derniersStagiaires = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    Nom = c.Stagiaire.Nom,
                    Prenom = c.Stagiaire.Prenom,
                    Email = c.Stagiaire.Email,
                    DateNaissance = c.Stagiaire.DateNaissance,
                })
                .ToList();

            // 🏆 Formations populaires
            var totalFormations = _context.Formations.Count();

            var formationsPopulaires = _context.Formations
                .Select(f => new
                {
                    Titre = f.Titre,
                    NombreCertificats = f.Certificats.Count
                })
                .OrderByDescending(f => f.NombreCertificats)
                .Skip((pageFormation - 1) * pageSizeFormation)
                .Take(pageSizeFormation)
                .ToList();

            // ✅ Taux de complétion
            int stagiairesAvecCertificat = _context.Stagiaires.Count(s => s.Certificats.Any());
            double tauxCompletion = stagiaireCount > 0
                ? (stagiairesAvecCertificat * 100.0) / stagiaireCount
                : 0;

            // ViewBag
            ViewBag.StagiaireCount = stagiaireCount;
            ViewBag.FormationCount = formationCount;
            ViewBag.CertificatCount = certificatCount;
            ViewBag.EncadrantCount = EncadrantCount;
            ViewBag.TauxCompletion = tauxCompletion.ToString("0.0");

            ViewBag.DerniersStagiaires = derniersStagiaires;
            ViewBag.FormationsPopulaires = formationsPopulaires;

            // Infos pour pagination
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalStagiaires / pageSize);

            ViewBag.CurrentPageFormation = pageFormation;
            ViewBag.TotalPagesFormation = (int)Math.Ceiling((double)totalFormations / pageSizeFormation);


            return View();
        }

    }
}
