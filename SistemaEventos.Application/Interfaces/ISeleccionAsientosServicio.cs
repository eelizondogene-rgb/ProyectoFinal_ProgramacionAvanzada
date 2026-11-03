using SistemaEventos.Application.DTOs;

namespace SistemaEventos.Application.Interfaces
{
    public interface ISeleccionAsientosServicio
    {
        Task<Respuesta<MapaAsientosDto>> ObtenerMapaAsync(
            int eventoId,
            IReadOnlyCollection<string>? posiciones,
            CancellationToken cancellationToken = default);

        Task<Respuesta<ResumenSeleccionDto>> ResumirSeleccionAsync(
            SeleccionAsientosDto seleccion,
            CancellationToken cancellationToken = default);
    }
}
