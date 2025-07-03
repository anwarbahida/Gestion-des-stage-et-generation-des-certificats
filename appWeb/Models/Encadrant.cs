using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb.Models
{
    [Table("Encadrants")]
    public class Encadrant
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est requis.")]
        public string Prenom { get; set; } = string.Empty;

        [EmailAddress]
        [Required(ErrorMessage = "L'email est requis.")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Telephone { get; set; } = string.Empty;

        // Clé étrangère unique vers Formation
        public int FormationId { get; set; }    // clé étrangère obligatoire

        public required Formation Formation { get; set; }
        

    }
}
