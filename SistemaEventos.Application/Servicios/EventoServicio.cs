using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Application.Servicios
{
    public class EventoServicio : IEventoServicio
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly AutoMapper.IMapper _mapper;

        public EventoServicio(IEventoRepository eventoRepository, AutoMapper.IMapper mapper)
        {
            _eventoRepository = eventoRepository;
            _mapper = mapper;
        }

        public async Task<List<EventoDto>> ObtenerEventosAsync(CancellationToken cancellationToken = default)
        {
            var eventos = await _eventoRepository.GetAllEventosAsync(cancellationToken);
            return _mapper.Map<List<EventoDto>>(eventos);
        }

        public async Task<EventoDto?> ObtenerEventoPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var evento = await _eventoRepository.GetEventoByIdAsync(id, cancellationToken);
            return _mapper.Map<EventoDto?>(evento);
        }

        public async Task<Respuesta<EventoDto>> CrearEventoAsync(EventoDto evento, CancellationToken cancellationToken = default)
        {
            var respuesta = new Respuesta<EventoDto>();
            var eventoEntity = _mapper.Map<Evento>(evento);

            if (!await _eventoRepository.CreateEventoAsync(eventoEntity, cancellationToken))
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "No se pudo crear el evento.";
                respuesta.Codigo = 500;
                return respuesta;
            }

            respuesta.Dato = _mapper.Map<EventoDto>(eventoEntity);
            return respuesta;
        }

        public async Task<Respuesta<EventoDto>> ActualizarEventoAsync(EventoDto evento, CancellationToken cancellationToken = default)
        {
            var respuesta = new Respuesta<EventoDto>();
            var eventoEntity = _mapper.Map<Evento>(evento);

            if (!await _eventoRepository.UpdateEventoAsync(eventoEntity, cancellationToken))
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "No se pudo actualizar el evento.";
                respuesta.Codigo = 500;
                return respuesta;
            }

            respuesta.Dato = _mapper.Map<EventoDto>(eventoEntity);
            return respuesta;
        }

        public async Task<Respuesta<EventoDto>> EliminarEventoAsync(int id, CancellationToken cancellationToken = default)
        {
            var respuesta = new Respuesta<EventoDto>();

            if (!await _eventoRepository.DeleteEventoAsync(id, cancellationToken))
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "No se pudo eliminar el evento.";
                respuesta.Codigo = 500;
                return respuesta;
            }

            return respuesta;
        }


        }
    }
