namespace SistemaEventos.Domain.Entidades
{
    public class Boleto
    {
        public int Id { get; set; }

        public int CompraId { get; set; }

        public int AsientoId { get; set; }

        public int EventoId { get; set; }

        public Compra Compra { get; set; } = null!;

        public Asiento Asiento { get; set; } = null!;
    }
}
