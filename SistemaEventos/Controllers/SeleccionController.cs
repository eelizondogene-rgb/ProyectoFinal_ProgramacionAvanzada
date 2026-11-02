using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;

namespace SistemaEventos.Controllers
{
    [AllowAnonymous]
    public class SeleccionController : Controller
    {
        private readonly ISeleccionAsientosServicio _seleccionAsientosServicio;

        public SeleccionController(ISeleccionAsientosServicio seleccionAsientosServicio)
        {
            _seleccionAsientosServicio = seleccionAsientosServicio;
        }

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
