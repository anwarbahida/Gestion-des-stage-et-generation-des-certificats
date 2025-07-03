using Microsoft.AspNetCore.Identity;

namespace appWeb.Models
{
    public class Admin : IdentityUser
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;

        public string Entreprise { get; set; } = string.Empty;

        public string LogoPath { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
    }

}
