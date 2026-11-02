using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Infrastructure.Datos.Configuraciones
{
    public class AsientoConfiguracion : IEntityTypeConfiguration<Asiento>
    {
        public void Configure(EntityTypeBuilder<Asiento> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.EventoId).IsRequired();
            builder.Property(a => a.Fila).IsRequired();
            builder.Property(a => a.Columna).IsRequired();

            builder.HasIndex(a => new { a.EventoId, a.Fila, a.Columna })
                .IsUnique();

            builder.HasAlternateKey(a => new { a.Id, a.EventoId });

            builder.HasOne(a => a.Evento)
                .WithMany()
                .HasForeignKey(a => a.EventoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
