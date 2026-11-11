namespace SistemaEventos.Application.DTOs
{
    public class CompraConfirmadaDto
    {
        public int CompraId { get; set; }

        public int EventoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public int PrecioEntrada { get; set; }

        public int Total { get; set; }

        public List<string> Posiciones { get; set; } = new();
    }
}
