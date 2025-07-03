using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appWeb.Data;

public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string type, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            ViewBag.Message = "Veuillez entrer un terme de recherche.";
            return View("ResultEmpty");
        }

        switch (type?.ToLower())
        {
            case "stagiaire":
                var stagiaires = _context.Stagiaires
                    .Where(s => s.Nom.Contains(query) || s.Prenom.Contains(query))
                    .ToList();
                ViewBag.Type = "Stagiaires";
                return View("ResultStagiaires", stagiaires);

            case "formation":
                var formations = _context.Formations
                    .Where(f => f.Titre.Contains(query) || f.Description.Contains(query))
                    .ToList();
                ViewBag.Type = "Formations";
                return View("ResultFormations", formations);

            case "encadrant":
                var encadrants = _context.Encadrants
                    .Where(e => e.Nom.Contains(query) || e.Prenom.Contains(query))
                    .ToList();
                ViewBag.Type = "Encadrants";
                return View("ResultEncadrants", encadrants);

            case "certificat":
                var certificats = _context.Certificats
                    .Include(c => c.Stagiaire)
                    .Include(c => c.Formation)
                    .Where(c => c.Stagiaire.Nom.Contains(query) ||
                                c.Stagiaire.Prenom.Contains(query) ||
                                c.Formation.Titre.Contains(query))
                    .ToList();
                ViewBag.Type = "Certificats";
                return View("ResultCertificats", certificats);

            default:
                ViewBag.Message = "Type de recherche invalide.";
                return View("ResultEmpty");
        }
    }
}
