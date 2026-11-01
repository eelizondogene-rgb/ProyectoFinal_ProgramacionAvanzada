using SistemaEventos.Application.DTOs;

namespace SistemaEventos.Application.Interfaces
{
    public interface IEventoServicio
    {
        Task<List<EventoDto>> ObtenerEventosAsync(CancellationToken cancellationToken = default);
        Task<EventoDto?> ObtenerEventoPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Respuesta<EventoDto>> CrearEventoAsync(EventoDto evento, CancellationToken cancellationToken = default);
        Task<Respuesta<EventoDto>> ActualizarEventoAsync(EventoDto evento, CancellationToken cancellationToken = default);
        Task<Respuesta<EventoDto>> EliminarEventoAsync(int id, CancellationToken cancellationToken = default);
    }
}