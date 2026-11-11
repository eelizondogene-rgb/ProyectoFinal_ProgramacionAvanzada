using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;
using SistemaEventos.Domain.Entidades;

namespace SistemaEventos.Application.Servicios
{
    public class CompraServicio : ICompraServicio
    {
        private readonly ISeleccionAsientosServicio _seleccionAsientosServicio;
        private readonly ICompraRepository _compraRepository;

        public CompraServicio(
            ISeleccionAsientosServicio seleccionAsientosServicio,
            ICompraRepository compraRepository)
        {
            _seleccionAsientosServicio = seleccionAsientosServicio;
            _compraRepository = compraRepository;
        }

        public async Task<Respuesta<CompraConfirmadaDto>> ConfirmarAsync(
            SeleccionAsientosDto seleccion,
            int usuarioId,
            CancellationToken cancellationToken = default)
        {
            if (usuarioId <= 0)
            {
                return Error("El usuario autenticado no es válido.", 400);
            }

            if (seleccion is null)
            {
                return Error("La selección es obligatoria.", 400);
            }

            var resumen = await _seleccionAsientosServicio.ResumirSeleccionAsync(
                seleccion,
                cancellationToken);

            if (!resumen.EsCorrecto || resumen.Dato is null)
            {
                return Error(
                    string.IsNullOrWhiteSpace(resumen.Mensaje)
                        ? "No se pudo confirmar la compra."
                        : resumen.Mensaje,
                    resumen.Codigo == 0 ? 400 : resumen.Codigo);
            }

            var vendidos = await _compraRepository.ObtenerAsientosVendidosAsync(
                resumen.Dato.EventoId,
                cancellationToken);

            var ocupados = vendidos
                .Select(asiento => (asiento.Fila, asiento.Columna))
                .ToHashSet();

            var boletos = new List<Boleto>(resumen.Dato.Posiciones.Count);

            foreach (var posicion in resumen.Dato.Posiciones)
            {
                if (!TryParsePosicion(posicion, out var fila, out var columna))
                {
                    return Error(
                        "La selección contiene una posición que no se puede confirmar.",
                        400);
                }

                if (ocupados.Contains((fila, columna)))
                {
                    return Error(
                        "Uno o más asientos ya no están disponibles.",
                        409);
                }

                boletos.Add(new Boleto
                {
                    Asiento = new Asiento
                    {
                        Fila = fila,
                        Columna = columna
                    }
                });
            }

            int total;

            try
            {
                total = checked(boletos.Count * resumen.Dato.PrecioEntrada);
            }
            catch (OverflowException)
            {
                return Error(
                    "El total de la selección supera el valor permitido.",
                    400);
            }

            var compra = new Compra
            {
                UsuarioId = usuarioId,
                EventoId = resumen.Dato.EventoId,
                Fecha = DateTime.UtcNow,
                Total = total,
                Boletos = boletos
            };

            var guardada = await _compraRepository.GuardarCompraAsync(
                compra,
                cancellationToken);

            if (guardada is null)
            {
                return Error(
                    "Uno o más asientos ya no están disponibles.",
                    409);
            }

            return new Respuesta<CompraConfirmadaDto>
            {
                Dato = new CompraConfirmadaDto
                {
                    CompraId = guardada.Id,
                    EventoId = guardada.EventoId,
                    Nombre = resumen.Dato.Nombre,
                    Cantidad = boletos.Count,
                    PrecioEntrada = resumen.Dato.PrecioEntrada,
                    Total = guardada.Total,
                    Posiciones = resumen.Dato.Posiciones.ToList()
                }
            };
        }

        private static bool TryParsePosicion(
            string? posicion,
            out int fila,
            out int columna)
        {
            fila = 0;
            columna = 0;

            if (string.IsNullOrWhiteSpace(posicion))
            {
                return false;
            }

            var partes = posicion.Split('-');

            if (partes.Length != 2)
            {
                return false;
            }

            return int.TryParse(partes[0].Trim(), out fila)
                && int.TryParse(partes[1].Trim(), out columna);
        }

        private static Respuesta<CompraConfirmadaDto> Error(string mensaje, int codigo)
        {
            return new Respuesta<CompraConfirmadaDto>
            {
                EsCorrecto = false,
                Mensaje = mensaje,
                Codigo = codigo
            };
        }
    }
}
