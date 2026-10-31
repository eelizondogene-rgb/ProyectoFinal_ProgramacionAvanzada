
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEventos.Application.Interfaces;

namespace SistemaEventos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;

        public UsuarioController(IUsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioServicio.ObtenerTodosAsync();

            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var usuario = await _usuarioServicio.ObtenerPorIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
    }
}
