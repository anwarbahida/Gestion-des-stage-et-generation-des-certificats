using System.Collections.Generic;

namespace appWeb.Models
{
    public class Formation
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        public ICollection<Certificat> Certificats { get; set; } = new List<Certificat>();

        // Navigation vers Encadrant (relation 1-1 inverse)
        public required Encadrant Encadrant { get; set; }

        
    }
}
