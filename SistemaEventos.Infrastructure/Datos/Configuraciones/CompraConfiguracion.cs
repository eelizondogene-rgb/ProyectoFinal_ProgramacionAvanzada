using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Infrastructure.Datos.Configuraciones
{
    public class CompraConfiguracion : IEntityTypeConfiguration<Compra>
    {
        public void Configure(EntityTypeBuilder<Compra> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UsuarioId).IsRequired();
            builder.Property(c => c.EventoId).IsRequired();
            builder.Property(c => c.Fecha).IsRequired();
            builder.Property(c => c.Total).IsRequired();

            builder.HasAlternateKey(c => new { c.Id, c.EventoId });

            builder.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Evento)
                .WithMany()
                .HasForeignKey(c => c.EventoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Boletos)
                .WithOne(b => b.Compra)
                .HasForeignKey(b => new { b.CompraId, b.EventoId })
                .HasPrincipalKey(c => new { c.Id, c.EventoId })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
