using SistemaEventos.Application.DTOs;

namespace SistemaEventos.Application.Interfaces
{
    public interface ICompraServicio
    {
        Task<Respuesta<CompraConfirmadaDto>> ConfirmarAsync(
            SeleccionAsientosDto seleccion,
            int usuarioId,
            CancellationToken cancellationToken = default);
    }
}
