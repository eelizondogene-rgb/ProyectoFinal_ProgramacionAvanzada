using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Infrastructure.Datos.Configuraciones
{
    public class BoletoConfiguracion : IEntityTypeConfiguration<Boleto>
    {
        public void Configure(EntityTypeBuilder<Boleto> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.CompraId).IsRequired();
            builder.Property(b => b.AsientoId).IsRequired();
            builder.Property(b => b.EventoId).IsRequired();

            builder.HasIndex(b => b.AsientoId)
                .IsUnique();

            builder.HasOne(b => b.Asiento)
                .WithMany()
                .HasForeignKey(b => new { b.AsientoId, b.EventoId })
                .HasPrincipalKey(a => new { a.Id, a.EventoId })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
