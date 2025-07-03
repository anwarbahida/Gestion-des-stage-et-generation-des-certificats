namespace appWeb.Models
{
    public class Stagiaire
    {
        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateNaissance { get; set; }

        public ICollection<Certificat> Certificats { get; set; } = new List<Certificat>();

        

    }

}
