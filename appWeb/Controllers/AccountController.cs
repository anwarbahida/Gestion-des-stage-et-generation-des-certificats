using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using appWeb.Models; 

public class AccountController : Controller
{
    private readonly UserManager<Admin> _userManager;
    private readonly SignInManager<Admin> _signInManager;

    public AccountController(UserManager<Admin> userManager, SignInManager<Admin> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Le nouveau mot de passe et la confirmation ne correspondent pas.");
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            ViewBag.Message = "Mot de passe modifié avec succès.";
            return View();
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }
    }
}
