
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaEventos.Domain.Entidades;
using SistemaEventos.Infrastructure.Datos;

namespace SistemaEventos
{
    public static class CrearAdministrador
    {
        public static async Task EjecutarAsync(
            ApplicationDbContext context,
            IPasswordHasher<Usuario> passwordHasher)
        {
            const string correo = "admin@sistemaeventos.com";
            const string contrasena = "Admin123*";

            var existente = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (existente != null)
            {
                Console.WriteLine("El administrador ya existe.");
                return;
            }

            var administrador = new Usuario
            {
                Nombre = "Administrador",
                Apellido = "Sistema",
                Correo = correo,
                Rol = "Administrador",
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };

            administrador.PasswordHash = passwordHasher.HashPassword(
                administrador,
                contrasena
            );

            context.Usuarios.Add(administrador);

            await context.SaveChangesAsync();

            Console.WriteLine("Administrador creado correctamente.");
        }
    }
}
