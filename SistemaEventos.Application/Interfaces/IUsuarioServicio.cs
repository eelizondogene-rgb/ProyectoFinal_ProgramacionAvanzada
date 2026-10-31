
using SistemaEventos.Application.DTOs;

namespace SistemaEventos.Application.Interfaces
{
    public interface IUsuarioServicio
    {
        Task<List<UsuarioDto>> ObtenerTodosAsync();

        Task<UsuarioDto?> ObtenerPorIdAsync(int id);

        Task<UsuarioDto?> RegistrarAsync(RegistroUsuarioDto registro);

        Task<UsuarioDto?> ValidarCredencialesAsync(LoginDto login);
    }
}
