
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace appWeb.Models
{
    public class Certificat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le champ Stagiaire est requis.")]
        [ForeignKey("Stagiaire")]
        public int StagiaireId { get; set; }

        [Required(ErrorMessage = "Le champ Formation est requis.")]
        [ForeignKey("Formation")]
        public int FormationId { get; set; }

        [Required(ErrorMessage = "La date de génération est requise.")]
        [DataType(DataType.Date)]
        public DateTime DateGeneration { get; set; }

        // Navigation properties
        public Stagiaire Stagiaire { get; set; } = default!;
        public Formation Formation { get; set; } = default!;
        
    }

}
