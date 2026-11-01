using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;

namespace SistemaEventos.Controllers
{
    public class EventoController : Controller
    {
        private readonly IEventoServicio _eventoServicio;

        public EventoController(IEventoServicio eventoServicio) 
        {
            _eventoServicio = eventoServicio;
        }


        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ObtenerEventos(CancellationToken cancellationToken)
        {
            var eventos = await _eventoServicio.ObtenerEventosAsync(cancellationToken);
            return Json(eventos);
        }

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

    }
}