using Microsoft.EntityFrameworkCore;
using SistemaEventos.Application.Interfaces;
using SistemaEventos.Domain.Entidades;
using SistemaEventos.Infrastructure.Datos;

namespace SistemaEventos.Infrastructure.Repositorios
{
    public class EventoRepository : IEventoRepository
    {
        private readonly ApplicationDbContext _context;

        public EventoRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<List<Evento>> GetAllEventosAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Eventos
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Evento?> GetEventoByIdAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Eventos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<bool> CreateEventoAsync(
            Evento evento, 
            CancellationToken cancellationToken = default)
        {

            await _context.Eventos.AddAsync(evento, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> UpdateEventoAsync(
            Evento evento, 
            CancellationToken cancellationToken = default)
        {
            var eventoExistente = await _context.Eventos
                .FirstOrDefaultAsync(e => e.Id == evento.Id, cancellationToken);

            if (eventoExistente is null)
            {
                return false;
            }

            eventoExistente.Nombre = evento.Nombre;

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteEventoAsync(
            int id, 
            CancellationToken cancellationToken = default)
        {
            var evento = await _context.Eventos
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (evento == null)
            {
                return false; 
            }

            _context.Eventos.Remove(evento);
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}