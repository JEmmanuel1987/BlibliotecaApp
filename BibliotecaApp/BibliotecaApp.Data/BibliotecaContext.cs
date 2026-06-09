using BibliotecaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Data
{
    public class BibliotecaContext : DbContext
    {
        // ─── DbSets ────────────────────────────────────────────────────────────
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        // ─── Cadena de conexión ────────────────────────────────────────────────
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
                @"Server=localhost\SQLEXPRESS;Database=BibliotecaDB;Trusted_Connection=True;TrustServerCertificate=True;"
            );
        }

        // ─── Configuración del modelo ──────────────────────────────────────────
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Libro
            modelBuilder.Entity<Libro>(e =>
            {
                e.HasKey(l => l.LibroId);
                e.Property(l => l.ISBN).HasMaxLength(20).IsRequired();
                e.Property(l => l.Titulo).HasMaxLength(200).IsRequired();
                e.Property(l => l.Autor).HasMaxLength(150).IsRequired();
                e.Property(l => l.Editorial).HasMaxLength(100);
                e.HasIndex(l => l.ISBN).IsUnique();
                e.HasOne(l => l.Categoria)
                 .WithMany(c => c.Libros)
                 .HasForeignKey(l => l.CategoriaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Socio
            modelBuilder.Entity<Socio>(e =>
            {
                e.HasKey(s => s.SocioId);
                e.Property(s => s.NroCarnet).HasMaxLength(20).IsRequired();
                e.Property(s => s.Nombre).HasMaxLength(100).IsRequired();
                e.Property(s => s.Apellido).HasMaxLength(100).IsRequired();
                e.Property(s => s.DeudaTotal).HasColumnType("decimal(10,2)");
                e.HasIndex(s => s.NroCarnet).IsUnique();
            });

            // Prestamo
            modelBuilder.Entity<Prestamo>(e =>
            {
                e.HasKey(p => p.PrestamoId);
                e.Property(p => p.MultaGenerada).HasColumnType("decimal(10,2)");
                e.Property(p => p.Estado).HasConversion<string>();
                e.HasOne(p => p.Libro)
                 .WithMany(l => l.Prestamos)
                 .HasForeignKey(p => p.LibroId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Socio)
                 .WithMany(s => s.Prestamos)
                 .HasForeignKey(p => p.SocioId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Categoria
            modelBuilder.Entity<Categoria>(e =>
            {
                e.HasKey(c => c.CategoriaId);
                e.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            });

            // ─── Datos semilla ────────────────────────────────────────────────
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { CategoriaId = 1, Nombre = "Ciencias Exactas",    Descripcion = "Matemáticas, Física, Química" },
                new Categoria { CategoriaId = 2, Nombre = "Ingeniería",           Descripcion = "Sistemas, Civil, Industrial" },
                new Categoria { CategoriaId = 3, Nombre = "Literatura",           Descripcion = "Novela, Cuento, Poesía" },
                new Categoria { CategoriaId = 4, Nombre = "Historia y Geografía", Descripcion = "Historia universal y boliviana" },
                new Categoria { CategoriaId = 5, Nombre = "Derecho",              Descripcion = "Derecho civil, penal, comercial" }
            );
        }
    }
}
