namespace SistemaEventos.Application.DTOs
{
    public class MapaAsientosDto
    {
        public int EventoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Capacidad { get; set; }

        public int PrecioEntrada { get; set; }

        public List<AsientoDisponibleDto> Asientos { get; set; } = new();
    }
}
