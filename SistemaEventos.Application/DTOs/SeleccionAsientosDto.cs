namespace SistemaEventos.Application.DTOs
{
    public class SeleccionAsientosDto
    {
        public int EventoId { get; set; }

        public List<string> Posiciones { get; set; } = new();
    }
}
