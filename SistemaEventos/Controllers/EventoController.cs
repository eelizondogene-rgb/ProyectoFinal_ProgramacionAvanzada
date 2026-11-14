using Microsoft.AspNetCore.Mvc;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;

namespace SistemaEventos.Controllers
{
    // Controlador del modulo de administracion de eventos. Solo coordina las peticiones HTTP,
    // toda la logica de negocio vive en la capa de aplicacion (IEventoServicio)
    public class EventoController : Controller
    {
        private readonly IEventoServicio _eventoServicio;

        public EventoController(IEventoServicio eventoServicio) // Inyección de dependencias
        {
            _eventoServicio = eventoServicio;
        }

        /// <summary>
        /// Muestra la pantalla de administración. Los datos de la tabla se cargan por AJAX desde ObtenerEventos.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Obtiene todos los eventos en formato JSON para la tabla.
        /// </summary>
        /// <param name="cancellationToken">Token que se cancela si el cliente aborta la petición.</param>
        /// <returns>Lista de eventos.</returns>
        public async Task<IActionResult> ObtenerEventos(CancellationToken cancellationToken)
        {
            var eventos = await _eventoServicio.ObtenerEventosAsync(cancellationToken);
            return Json(eventos);
        }

        /// <summary>
        /// Obtiene un evento por su id en formato JSON para cargar el formulario de edición.
        /// </summary>
        /// <param name="id">Id del evento.</param>
        /// <param name="cancellationToken">Token que se cancela si el cliente aborta la petición.</param>
        /// <returns>El evento encontrado o null.</returns>
        public async Task<IActionResult> ObtenerEventoPorId(int id, CancellationToken cancellationToken)
        {
            var evento = await _eventoServicio.ObtenerEventoPorIdAsync(id, cancellationToken);
            return Json(evento);
        }

        /// <summary>
        /// Crea un nuevo evento aplicando las reglas de negocio del servicio.
        /// </summary>
        /// <param name="evento">Datos del formulario (binding model).</param>
        /// <param name="cancellationToken">Token que se cancela si el cliente aborta la petición.</param>
        /// <returns>Objeto Respuesta con el resultado de la operación.</returns>
        [HttpPost]
        public async Task<IActionResult> CrearEvento(EventoDto evento, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = await _eventoServicio.CrearEventoAsync(evento, cancellationToken);
            return Json(respuesta);
        }

        /// <summary>
        /// Actualiza un evento existente aplicando las reglas de negocio del servicio.
        /// </summary>
        /// <param name="evento">Datos del formulario (binding model), incluye el Id.</param>
        /// <param name="cancellationToken">Token que se cancela si el cliente aborta la petición.</param>
        /// <returns>Objeto Respuesta con el resultado de la operación.</returns>
        [HttpPost]
        public async Task<IActionResult> ActualizarEvento(EventoDto evento, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = await _eventoServicio.ActualizarEventoAsync(evento, cancellationToken);
            return Json(respuesta);
        }

        /// <summary>
        /// Elimina un evento.
        /// </summary>
        /// <param name="id">Id del evento.</param>
        /// <param name="cancellationToken">Token que se cancela si el cliente aborta la petición.</param>
        /// <returns>Objeto Respuesta con el resultado de la operación.</returns>
        [HttpDelete]
        public async Task<IActionResult> EliminarEvento(int id, CancellationToken cancellationToken)
        {
            var respuesta = await _eventoServicio.EliminarEventoAsync(id, cancellationToken);
            return Json(respuesta);
        }
    }
}