using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourRide.Models;

namespace YourRide.Data
{
    public class ApplicationDbContext : IdentityDbContext<Korisnik>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        
        
        public DbSet<Voznja> Voznja { get; set; }
       
        public DbSet<Lokacija> Lokacija { get; set; }
        public DbSet<Ruta> Ruta { get; set; }
        public DbSet<Ocjena> Ocjena { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>().ToTable("AspNetUsers");
            modelBuilder.Entity<Voznja>().ToTable("Voznja");
           
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



            

           

            base.OnModelCreating(modelBuilder);
        }



    }
}
