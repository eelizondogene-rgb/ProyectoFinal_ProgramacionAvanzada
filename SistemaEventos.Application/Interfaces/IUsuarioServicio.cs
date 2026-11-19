
using SistemaEventos.Application.DTOs;

namespace SistemaEventos.Application.Interfaces
{
    public interface IUsuarioServicio
    {
        Task<List<UsuarioDto>> ObtenerTodosAsync();

        Task<UsuarioDto?> ObtenerPorIdAsync(int id);

        Task<UsuarioDto?> RegistrarAsync(RegistroUsuarioDto registro);

        Task<UsuarioDto?> ValidarCredencialesAsync(LoginDto login);

        Task<bool> ActualizarAsync(int id, ActualizarUsuarioDto usuario);

        Task<bool> CambiarRolAsync(int id, string rol);

        Task<bool> CambiarEstadoAsync(int id, bool activo);
    }
}
