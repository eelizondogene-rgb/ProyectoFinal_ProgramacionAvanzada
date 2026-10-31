
using Microsoft.AspNetCore.Identity;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Application.Servicios
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IUsuarioRepository _repository;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioServicio(
            IUsuarioRepository repository,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<UsuarioDto>> ObtenerTodosAsync()
        {
            var usuarios = await _repository.ObtenerTodosAsync();

            return usuarios.Select(MapearDto).ToList();
        }

        public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
        {
            var usuario = await _repository.ObtenerPorIdAsync(id);

            return usuario == null ? null : MapearDto(usuario);
        }

        public async Task<UsuarioDto?> RegistrarAsync(
            RegistroUsuarioDto registro)
        {
            if (registro.Contrasena != registro.ConfirmarContrasena)
                return null;

            if (registro.Contrasena.Length < 8)
                return null;

            var correo = registro.Correo.Trim().ToLowerInvariant();

            var existente = await _repository
                .ObtenerPorCorreoAsync(correo);

            if (existente != null)
                return null;

            var usuario = new Usuario
            {
                Nombre = registro.Nombre.Trim(),
                Apellido = registro.Apellido.Trim(),
                Correo = correo,
                Rol = "Cliente",
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };

            usuario.PasswordHash = _passwordHasher
                .HashPassword(usuario, registro.Contrasena);

            await _repository.AgregarAsync(usuario);

            return MapearDto(usuario);
        }

        public async Task<UsuarioDto?> ValidarCredencialesAsync(
            LoginDto login)
        {
            var correo = login.Correo.Trim().ToLowerInvariant();

            var usuario = await _repository
                .ObtenerPorCorreoAsync(correo);

            if (usuario == null || !usuario.Activo)
                return null;

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                login.Contrasena);

            if (resultado == PasswordVerificationResult.Failed)
                return null;

            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                usuario.PasswordHash = _passwordHasher
                    .HashPassword(usuario, login.Contrasena);

                await _repository.ActualizarAsync(usuario);
            }

            return MapearDto(usuario);
        }

        private static UsuarioDto MapearDto(Usuario usuario)
        {
            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Activo = usuario.Activo,
                FechaRegistro = usuario.FechaRegistro
            };
        }
    }
}
