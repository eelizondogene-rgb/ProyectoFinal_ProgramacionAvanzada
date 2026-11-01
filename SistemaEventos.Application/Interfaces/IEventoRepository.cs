using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Application.Interfaces
{
    public interface IEventoRepository
    {
        Task<List<Evento>> GetAllEventosAsync(CancellationToken cancellationToken = default);
        Task<Evento> GetEventoByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> CreateEventoAsync(Evento evento, CancellationToken cancellationToken = default);
        Task<bool> UpdateEventoAsync(Evento evento, CancellationToken cancellationToken = default);
        Task<bool> DeleteEventoAsync(int id, CancellationToken cancellationToken = default);
    }
}