namespace SistemaEventos.Domain.Entidades
{
    public class Asiento
    {
        public int Id { get; set; }

        public int EventoId { get; set; }

        public int Fila { get; set; }

        public int Columna { get; set; }

        public Evento Evento { get; set; } = null!;
    }
}
