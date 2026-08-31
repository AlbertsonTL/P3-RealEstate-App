using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entidades;

namespace RealEstateApp.Infrastructure.Data.Contexto
{
    public class AplicacionDbContext : DbContext
    {
        public AplicacionDbContext(DbContextOptions<AplicacionDbContext> options) : base(options) { }

        public DbSet<Propiedad> Propiedades { get; set; }
        public DbSet<TipoPropiedad> TiposPropiedades { get; set; }
        public DbSet<TipoVenta> TiposVentas { get; set; }
        public DbSet<Mejora> Mejoras { get; set; }
        public DbSet<ImagenPropiedad> ImagenesPropiedades { get; set; }
        public DbSet<PropiedadMejora> PropiedadesMejoras { get; set; }
        public DbSet<Oferta> Ofertas { get; set; }
        public DbSet<ChatMensaje> ChatMensajes { get; set; }
        public DbSet<PropiedadFavorita> PropiedadesFavoritas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Nombres de tablas
            modelBuilder.Entity<Propiedad>().ToTable("Propiedades");
            modelBuilder.Entity<TipoPropiedad>().ToTable("TiposPropiedades");
            modelBuilder.Entity<TipoVenta>().ToTable("TiposVentas");
            modelBuilder.Entity<Mejora>().ToTable("Mejoras");
            modelBuilder.Entity<ImagenPropiedad>().ToTable("ImagenesPropiedades");
            modelBuilder.Entity<PropiedadMejora>().ToTable("PropiedadesMejoras");
            modelBuilder.Entity<Oferta>().ToTable("Ofertas");
            modelBuilder.Entity<ChatMensaje>().ToTable("ChatMensajes");
            modelBuilder.Entity<PropiedadFavorita>().ToTable("PropiedadesFavoritas");

            // PK Compuestas
            modelBuilder.Entity<PropiedadMejora>().HasKey(pm => new { pm.PropiedadId, pm.MejoraId });
            modelBuilder.Entity<PropiedadFavorita>().HasKey(pf => new { pf.PropiedadId, pf.ClienteId });

            // Relaciones y configuraciones
            // Precisión decimal explícita para evitar truncamiento silencioso
            modelBuilder.Entity<Propiedad>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Propiedad>()
                .Property(p => p.TamañoMetros)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Oferta>()
                .Property(o => o.CifraOfertada)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Propiedad>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            modelBuilder.Entity<Propiedad>()
                .HasOne(p => p.TipoPropiedad)
                .WithMany(tp => tp.Propiedades)
                .HasForeignKey(p => p.TipoPropiedadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Propiedad>()
                .HasOne(p => p.TipoVenta)
                .WithMany(tv => tv.Propiedades)
                .HasForeignKey(p => p.TipoVentaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Oferta>()
                .HasOne(o => o.Propiedad)
                .WithMany(p => p.Ofertas)
                .HasForeignKey(o => o.PropiedadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ImagenPropiedad>()
                .HasOne(i => i.Propiedad)
                .WithMany(p => p.Imagenes)
                .HasForeignKey(i => i.PropiedadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropiedadMejora>()
                .HasOne(pm => pm.Propiedad)
                .WithMany(p => p.PropiedadesMejoras)
                .HasForeignKey(pm => pm.PropiedadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropiedadMejora>()
                .HasOne(pm => pm.Mejora)
                .WithMany(m => m.PropiedadesMejoras)
                .HasForeignKey(pm => pm.MejoraId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropiedadFavorita>()
                .HasOne(pf => pf.Propiedad)
                .WithMany(p => p.Favoritas)
                .HasForeignKey(pf => pf.PropiedadId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
