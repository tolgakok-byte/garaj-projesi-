using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;

namespace FleetManagementAPI
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options) { }

        public DbSet<Arac> Araclar { get; set; }
        public DbSet<Surucu> Suruculer { get; set; }
        public DbSet<GorevKayit> Gorevlendirmeler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SQL Tablo İsimleri Eşleme
            modelBuilder.Entity<Arac>().ToTable("Araclar");
            modelBuilder.Entity<Surucu>().ToTable("Suruculer");
            modelBuilder.Entity<GorevKayit>().ToTable("Gorevlendirmeler");

            base.OnModelCreating(modelBuilder);
        }
    }
}