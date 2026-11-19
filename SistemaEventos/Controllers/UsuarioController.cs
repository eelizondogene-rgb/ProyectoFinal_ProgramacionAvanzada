
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;
using System.Security.Claims;

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
                return NotFound();

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _usuarioServicio.ObtenerPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var modelo = new ActualizarUsuarioDto
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo
            };

            ViewBag.UsuarioId = id;

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            ActualizarUsuarioDto modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UsuarioId = id;
                return View(modelo);
            }

            var actualizado = await _usuarioServicio.ActualizarAsync(
                id, modelo);

            if (!actualizado)
            {
                ModelState.AddModelError("",
                    "No se pudo actualizar. Verifique que el correo no esté registrado por otro usuario.");

                ViewBag.UsuarioId = id;
                return View(modelo);
            }

            TempData["Mensaje"] = "Usuario actualizado correctamente.";

            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRol(int id, string rol)
        {
            var idActual = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (idActual == id.ToString() &&
                rol != "Administrador")
            {
                TempData["Error"] =
                    "No puedes quitarte tu propio rol de administrador.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            var cambiado = await _usuarioServicio.CambiarRolAsync(id, rol);

            TempData[cambiado ? "Mensaje" : "Error"] = cambiado
                ? "Rol actualizado correctamente."
                : "No se pudo actualizar el rol.";

            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            var idActual = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (idActual == id.ToString() && !activo)
            {
                TempData["Error"] =
                    "No puedes desactivar tu propia cuenta.";

                return RedirectToAction(nameof(Detalle), new { id });
            }

            var cambiado = await _usuarioServicio.CambiarEstadoAsync(
                id, activo);

            TempData[cambiado ? "Mensaje" : "Error"] = cambiado
                ? "Estado actualizado correctamente."
                : "No se pudo actualizar el estado.";

            return RedirectToAction(nameof(Detalle), new { id });
        }
    }
}
