using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace appWeb.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel(ILogger<ForgotPasswordModel> logger) : PageModel
    {
        private readonly ILogger<ForgotPasswordModel> _logger = logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "L'email est requis.")]
            [EmailAddress(ErrorMessage = "Format d'email invalide.")]
            public required string Email { get; set; }
        }

        public void OnGet()
        {
            // Si tu veux initialiser des valeurs, fais-le ici.
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Affiche les erreurs de validation
            }

            // 👉 Ici, tu peux intégrer la logique d'envoi de l'email de réinitialisation :
            // Par exemple : vérifier si l'utilisateur existe, générer un token, envoyer l'email...

            _logger.LogInformation("Demande de réinitialisation du mot de passe pour l'adresse : {Email}", Input.Email);

            // Pour cet exemple, redirigeons vers une page de confirmation :
            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
