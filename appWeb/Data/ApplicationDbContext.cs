using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using appWeb.Models;

namespace appWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<Admin>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Certificat> Certificats { get; set; }
        public DbSet<Stagiaire> Stagiaires { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Encadrant> Encadrants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Stagiaire>().ToTable("Stagiaire");
            modelBuilder.Entity<Formation>().ToTable("Formation");
            modelBuilder.Entity<Certificat>().ToTable("Certificat");
            modelBuilder.Entity<Encadrant>().ToTable("Encadrants");

            modelBuilder.Entity<Encadrant>()
                .HasOne(e => e.Formation)
                .WithOne(f => f.Encadrant)
                .HasForeignKey<Encadrant>(e => e.FormationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}
