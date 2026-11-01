using System.ComponentModel.DataAnnotations;

namespace SistemaEventos.Application.DTOs
{
    public class EventoDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public string Lugar { get; set; } = string.Empty;

        public int Capacidad { get; set; }

        public int PrecioEntrada { get; set; }

    }
}