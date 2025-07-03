using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using appWeb.Models;

public class AdminController : Controller
{
    private readonly UserManager<Admin> _userManager;
    private readonly IWebHostEnvironment _env;

    public AdminController(UserManager<Admin> userManager, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _env = env;
    }


    public async Task<IActionResult> EditProfil()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfil(Admin model, IFormFile photo, IFormFile logo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        user.Nom = model.Nom;
        user.Prenom = model.Prenom;
        user.PhoneNumber = model.PhoneNumber;
        user.Entreprise = model.Entreprise;
        user.Ville = model.Ville;

        // PHOTO (image profil)
        if (photo != null && photo.Length > 0)
        {
            var photoFileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var photoPath = Path.Combine(_env.WebRootPath, "images", photoFileName);

            using (var stream = new FileStream(photoPath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            user.PhotoPath = "/images/" + photoFileName;
        }

        // LOGO (logo entreprise)
        if (logo != null && logo.Length > 0)
        {
            var logoDirectory = Path.Combine(_env.WebRootPath, "logos");
            Directory.CreateDirectory(logoDirectory); // Crée le dossier s'il n'existe pas

            var logoFileName = Guid.NewGuid() + Path.GetExtension(logo.FileName);
            var logoFullPath = Path.Combine(logoDirectory, logoFileName);

            using (var stream = new FileStream(logoFullPath, FileMode.Create))
            {
                await logo.CopyToAsync(stream);
            }

            user.LogoPath = "/logos/" + logoFileName;
        }

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Profil");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(user);
    }


    public async Task<IActionResult> Profil()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account"); // ou une autre redirection
        }

        ViewBag.UserName = user.UserName;
        ViewBag.Email = user.Email;
        ViewBag.Phone = user.PhoneNumber;
        ViewBag.Nom = user.Nom;
        ViewBag.Prenom = user.Prenom;
        ViewBag.PhotoPath = string.IsNullOrEmpty(user.PhotoPath) ? "/images/admin.jpg" : user.PhotoPath;
        ViewBag.LogoPath = user.LogoPath ?? "";
        ViewBag.Entreprise = user.Entreprise;
        ViewBag.Ville = user.Ville ?? ""; // Assurez-vous que la propriété Ville est initialisée

        return View();
    }

}
