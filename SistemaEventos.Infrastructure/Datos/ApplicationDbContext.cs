
using Microsoft.EntityFrameworkCore;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Infrastructure.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Correo)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(u => u.Correo)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Rol)
                    .IsRequired()
                    .HasMaxLength(20);
            });
        }
    }
}
