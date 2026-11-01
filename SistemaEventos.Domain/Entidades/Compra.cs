namespace SistemaEventos.Domain.Entidades
{
    public class Compra
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int EventoId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public int Total { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public Evento Evento { get; set; } = null!;

        public ICollection<Boleto> Boletos { get; set; } = new List<Boleto>();
    }
}
