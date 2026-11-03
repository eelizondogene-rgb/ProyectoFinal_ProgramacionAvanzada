using AutoMapper;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Application.Mapping
{
    public class MapeoClases : Profile
    {
        public MapeoClases()
        {
            CreateMap<Evento, EventoDto>().ReverseMap();
        }
    }
}
