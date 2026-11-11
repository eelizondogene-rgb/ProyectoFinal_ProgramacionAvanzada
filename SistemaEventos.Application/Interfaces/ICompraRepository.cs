using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Application.Interfaces
{
    public interface ICompraRepository
    {
        Task<List<Asiento>> ObtenerAsientosVendidosAsync(
            int eventoId,
            CancellationToken cancellationToken = default);

        Task<Compra?> GuardarCompraAsync(
            Compra compra,
            CancellationToken cancellationToken = default);
    }
}
