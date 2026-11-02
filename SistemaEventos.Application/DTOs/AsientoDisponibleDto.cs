namespace SistemaEventos.Application.DTOs
{
    public class AsientoDisponibleDto
    {
        public int Fila { get; set; }

        public int Columna { get; set; }

        public string Etiqueta { get; set; } = string.Empty;

        public bool Seleccionado { get; set; }
    }
}
