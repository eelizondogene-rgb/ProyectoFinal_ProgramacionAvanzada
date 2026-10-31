
using System;

namespace SistemaEventos.Application.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
