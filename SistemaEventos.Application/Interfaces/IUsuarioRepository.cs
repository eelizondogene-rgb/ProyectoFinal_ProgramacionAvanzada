
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorIdAsync(int id);

        Task<Usuario?> ObtenerPorCorreoAsync(string correo);

        Task<List<Usuario>> ObtenerTodosAsync();

        Task AgregarAsync(Usuario usuario);

        Task ActualizarAsync(Usuario usuario);
    }
}
