namespace SistemaEventos.Domain.Entidades
{
    public class Evento
    {
        public const int CapacidadMinima = 1;

        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public string Lugar { get; set; } = null!;

        public DateTime? FechaEvento { get; set; }

        public int Capacidad { get; set; }

        public int PrecioEntrada { get; set; }

        public DateTime? FechaCreacion { get; set; }




        public bool ValidarReglaNegocioFechaFutura()
        {
            if (FechaCreacion.HasValue && FechaCreacion.Value.Date == DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }


        public bool ValidarReglaNegocioCapacidad()
        {
            if (Capacidad >= CapacidadMinima)
            {
                return true;
            }
            return false;
        }


        public bool ValidarReglaNegocioPrecio()
        {
            if (PrecioEntrada >= 0)
            {
                return true;
            }
            return false;
        }

        public bool ValidarReglasDeNegocio()
        {
            return ValidarReglaNegocioFechaFutura()
                && ValidarReglaNegocioCapacidad()
                && ValidarReglaNegocioPrecio();
        }
    }
}