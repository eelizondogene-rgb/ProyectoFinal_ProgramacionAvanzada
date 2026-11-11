using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;
using SistemaEventos.Application.Servicios;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Tests
{
    public class CompraServicioPruebas
    {
        [Fact]
        public async Task ConfirmarAsync_AsientoVendido_Devuelve409SinGuardar()
        {
            var seleccion = new SeleccionAsientosServicioFalso();
            var compras = new CompraRepositoryFalso();
            var servicio = new CompraServicio(seleccion, compras);

            var respuesta = await servicio.ConfirmarAsync(
                new SeleccionAsientosDto
                {
                    EventoId = 1,
                    Posiciones = new List<string> { "1-1" }
                },
                usuarioId: 1);

            Assert.False(respuesta.EsCorrecto);
            Assert.Equal(409, respuesta.Codigo);
            Assert.Equal(
                "Uno o más asientos ya no están disponibles.",
                respuesta.Mensaje);
            Assert.Equal(0, compras.Guardados);
        }

        private sealed class SeleccionAsientosServicioFalso : ISeleccionAsientosServicio
        {
            public Task<Respuesta<MapaAsientosDto>> ObtenerMapaAsync(
                int eventoId,
                IReadOnlyCollection<string>? posiciones,
                CancellationToken cancellationToken = default)
            {
                throw new NotSupportedException(
                    "Esta prueba no debe consultar el mapa.");
            }

            public Task<Respuesta<ResumenSeleccionDto>> ResumirSeleccionAsync(
                SeleccionAsientosDto seleccion,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new Respuesta<ResumenSeleccionDto>
                {
                    Dato = new ResumenSeleccionDto
                    {
                        EventoId = seleccion.EventoId,
                        Nombre = "Evento de prueba",
                        PrecioEntrada = 15000,
                        Cantidad = seleccion.Posiciones.Count,
                        Total = 15000,
                        Posiciones = seleccion.Posiciones.ToList()
                    }
                });
            }
        }

        private sealed class CompraRepositoryFalso : ICompraRepository
        {
            public int Guardados { get; private set; }

            public Task<List<Asiento>> ObtenerAsientosVendidosAsync(
                int eventoId,
                CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new List<Asiento>
                {
                    new Asiento
                    {
                        Id = 1,
                        EventoId = eventoId,
                        Fila = 1,
                        Columna = 1
                    }
                });
            }

            public Task<Compra?> GuardarCompraAsync(
                Compra compra,
                CancellationToken cancellationToken = default)
            {
                Guardados++;
                return Task.FromResult<Compra?>(null);
            }
        }
    }
}
