using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourRide.Models;

namespace YourRide.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Korisnik> Korisnik { get; set; }
        
        public DbSet<Voznja> Voznja { get; set; }
        public DbSet<Notifikacija> Notifikacija { get; set; }
        public DbSet<Lokacija> Lokacija { get; set; }
        public DbSet<Ruta> Ruta { get; set; }
        public DbSet<Ocjena> Ocjena { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<Voznja>().ToTable("Voznja");
            modelBuilder.Entity<Notifikacija>().ToTable("Notifikacija");
            modelBuilder.Entity<Lokacija>().ToTable("Lokacija");
            modelBuilder.Entity<Ruta>().ToTable("Ruta");
            modelBuilder.Entity<Ocjena>().ToTable("Ocjena");
            

            modelBuilder.Entity<Ruta>()
        .HasOne(r => r.PocetnaLokacija)
        .WithMany()
        .HasForeignKey(r => r.PocetnaLokacijaId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ruta>()
                .HasOne(r => r.KrajnjaLokacija)
                .WithMany()
                .HasForeignKey(r => r.KrajnjaLokacijaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voznja>()
    .HasOne(v => v.Putnik)
    .WithMany()
    .HasForeignKey(v => v.PutnikId)
    .OnDelete(DeleteBehavior.Restrict); // sprječava kaskadno brisanje

            modelBuilder.Entity<Voznja>()
                .HasOne(v => v.Vozac)
                .WithMany()
                .HasForeignKey(v => v.VozacId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voznja>()
    .Property(v => v.Cijena)
    .HasPrecision(10, 2);

            modelBuilder.Entity<Notifikacija>()
        .HasOne(n => n.Posiljalac)
        .WithMany()
        .HasForeignKey(n => n.PosiljalacId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notifikacija>()
                .HasOne(n => n.Primalac)
                .WithMany()
                .HasForeignKey(n => n.PrimalacId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }



    }
}
