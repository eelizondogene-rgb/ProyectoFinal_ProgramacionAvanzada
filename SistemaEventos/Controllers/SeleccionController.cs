using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;

namespace SistemaEventos.Controllers
{
    public class SeleccionController : Controller
    {
        private readonly ISeleccionAsientosServicio _seleccionAsientosServicio;
        private readonly ICompraServicio _compraServicio;

        public SeleccionController(
            ISeleccionAsientosServicio seleccionAsientosServicio,
            ICompraServicio compraServicio)
        {
            _seleccionAsientosServicio = seleccionAsientosServicio;
            _compraServicio = compraServicio;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(
            int eventoId,
            CancellationToken cancellationToken)
        {
            var respuesta = await _seleccionAsientosServicio.ObtenerMapaAsync(
                eventoId,
                posiciones: null,
                cancellationToken);

            if (!respuesta.EsCorrecto || respuesta.Dato is null)
            {
                return ErrorDeSeleccion(respuesta.Codigo, respuesta.Mensaje);
            }

            return View(respuesta.Dato);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            SeleccionAsientosDto seleccion,
            CancellationToken cancellationToken)
        {
            if (seleccion is null)
            {
                return BadRequest("La selección es obligatoria.");
            }

            var respuesta = await _seleccionAsientosServicio.ObtenerMapaAsync(
                seleccion.EventoId,
                seleccion.Posiciones,
                cancellationToken);

            if (!respuesta.EsCorrecto || respuesta.Dato is null)
            {
                return ErrorDeSeleccion(respuesta.Codigo, respuesta.Mensaje);
            }

            return View(respuesta.Dato);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resumen(
            SeleccionAsientosDto seleccion,
            CancellationToken cancellationToken)
        {
            if (seleccion is null)
            {
                return BadRequest("La selección es obligatoria.");
            }

            var resumen = await _seleccionAsientosServicio.ResumirSeleccionAsync(
                seleccion,
                cancellationToken);

            if (resumen.EsCorrecto && resumen.Dato is not null)
            {
                return View(resumen.Dato);
            }

            ModelState.AddModelError(string.Empty, Mensaje(resumen.Mensaje));

            var mapa = await _seleccionAsientosServicio.ObtenerMapaAsync(
                seleccion.EventoId,
                seleccion.Posiciones,
                cancellationToken);

            if (!mapa.EsCorrecto || mapa.Dato is null)
            {
                return ErrorDeSeleccion(mapa.Codigo, mapa.Mensaje);
            }

            return View("Index", mapa.Dato);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(
            SeleccionAsientosDto seleccion,
            CancellationToken cancellationToken)
        {
            if (seleccion is null)
            {
                return BadRequest("La selección es obligatoria.");
            }

            var identificador = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(identificador, out var usuarioId) || usuarioId <= 0)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo identificar al usuario autenticado.");

                return await VolverASeleccionarAsync(seleccion, cancellationToken);
            }

            var confirmacion = await _compraServicio.ConfirmarAsync(
                seleccion,
                usuarioId,
                cancellationToken);

            if (confirmacion.EsCorrecto && confirmacion.Dato is not null)
            {
                return View("Confirmacion", confirmacion.Dato);
            }

            ModelState.AddModelError(string.Empty, Mensaje(confirmacion.Mensaje));

            return await VolverASeleccionarAsync(seleccion, cancellationToken);
        }

        private async Task<IActionResult> VolverASeleccionarAsync(
            SeleccionAsientosDto seleccion,
            CancellationToken cancellationToken)
        {
            var mapa = await _seleccionAsientosServicio.ObtenerMapaAsync(
                seleccion.EventoId,
                seleccion.Posiciones,
                cancellationToken);

            if (!mapa.EsCorrecto || mapa.Dato is null)
            {
                return ErrorDeSeleccion(mapa.Codigo, mapa.Mensaje);
            }

            return View("Index", mapa.Dato);
        }

        private IActionResult ErrorDeSeleccion(int codigo, string? mensaje)
        {
            var texto = Mensaje(mensaje);

            if (codigo == StatusCodes.Status404NotFound)
            {
                return NotFound(texto);
            }

            return BadRequest(texto);
        }

        private static string Mensaje(string? mensaje)
        {
            return string.IsNullOrWhiteSpace(mensaje)
                ? "No se pudo procesar la selección."
                : mensaje;
        }
    }
}
