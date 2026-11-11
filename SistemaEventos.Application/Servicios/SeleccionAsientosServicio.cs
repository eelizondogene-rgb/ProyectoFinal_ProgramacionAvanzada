using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;

namespace SistemaEventos.Application.Servicios
{
    public class SeleccionAsientosServicio : ISeleccionAsientosServicio
    {
        private const int Filas = 10;
        private const int Columnas = 10;
        private const int MaximoAsientos = Filas * Columnas;

        private readonly IEventoServicio _eventoServicio;
        private readonly ICompraRepository _compraRepository;

        public SeleccionAsientosServicio(
            IEventoServicio eventoServicio,
            ICompraRepository compraRepository)
        {
            _eventoServicio = eventoServicio;
            _compraRepository = compraRepository;
        }

        public async Task<Respuesta<MapaAsientosDto>> ObtenerMapaAsync(
            int eventoId,
            IReadOnlyCollection<string>? posiciones,
            CancellationToken cancellationToken = default)
        {
            var preparacion = await PrepararSeleccionAsync<MapaAsientosDto>(
                eventoId,
                posiciones,
                exigirAlMenosUna: false,
                calcularTotal: false,
                cancellationToken);

            if (preparacion.Error is not null)
            {
                return preparacion.Error;
            }

            var vendidos = await _compraRepository.ObtenerAsientosVendidosAsync(
                preparacion.Evento!.Id,
                cancellationToken);

            var ocupados = vendidos
                .Select(asiento => new Coordenada(asiento.Fila, asiento.Columna))
                .ToHashSet();

            return Exito(CrearMapa(
                preparacion.Evento!,
                preparacion.Coordenadas!,
                ocupados));
        }

        public async Task<Respuesta<ResumenSeleccionDto>> ResumirSeleccionAsync(
            SeleccionAsientosDto seleccion,
            CancellationToken cancellationToken = default)
        {
            if (seleccion is null)
            {
                return Error<ResumenSeleccionDto>("La selección es obligatoria.", 400);
            }

            var preparacion = await PrepararSeleccionAsync<ResumenSeleccionDto>(
                seleccion.EventoId,
                seleccion.Posiciones,
                exigirAlMenosUna: true,
                calcularTotal: true,
                cancellationToken);

            if (preparacion.Error is not null)
            {
                return preparacion.Error;
            }

            var evento = preparacion.Evento!;
            var coordenadas = preparacion.Coordenadas!;

            return Exito(new ResumenSeleccionDto
            {
                EventoId = evento.Id,
                Nombre = evento.Nombre,
                PrecioEntrada = evento.PrecioEntrada,
                Cantidad = coordenadas.Count,
                Total = preparacion.Total,
                Posiciones = coordenadas.Select(CrearPosicion).ToList()
            });
        }

        private async Task<Preparacion<T>> PrepararSeleccionAsync<T>(
            int eventoId,
            IReadOnlyCollection<string>? posiciones,
            bool exigirAlMenosUna,
            bool calcularTotal,
            CancellationToken cancellationToken)
        {
            if (eventoId <= 0)
            {
                return Preparacion<T>.Fallida(
                    Error<T>("El identificador del evento debe ser mayor que cero.", 400));
            }

            var evento = await _eventoServicio.ObtenerEventoPorIdAsync(
                eventoId,
                cancellationToken);

            if (evento is null)
            {
                return Preparacion<T>.Fallida(
                    Error<T>("El evento no existe.", 404));
            }

            var coordenadas = new List<Coordenada>();
            var vistas = new HashSet<Coordenada>();

            foreach (var posicion in posiciones ?? Array.Empty<string>())
            {
                if (!TryParsePosicion(posicion, out var fila, out var columna))
                {
                    return Preparacion<T>.Fallida(
                        Error<T>($"La posición '{posicion}' no es válida. Use el formato fila-columna, por ejemplo 1-5.", 400));
                }

                if (fila < 1 || fila > Filas || columna < 1 || columna > Columnas)
                {
                    return Preparacion<T>.Fallida(
                        Error<T>($"La posición {fila}-{columna} está fuera del rango de 1 a {Filas}.", 400));
                }

                var coordenada = new Coordenada(fila, columna);

                if (!vistas.Add(coordenada))
                {
                    return Preparacion<T>.Fallida(
                        Error<T>($"La posición {CrearPosicion(coordenada)} está repetida.", 400));
                }

                coordenadas.Add(coordenada);
            }

            if (exigirAlMenosUna && coordenadas.Count == 0)
            {
                return Preparacion<T>.Fallida(
                    Error<T>("Debe seleccionar al menos un asiento.", 400));
            }

            if (coordenadas.Count > MaximoAsientos)
            {
                return Preparacion<T>.Fallida(
                    Error<T>($"No puede seleccionar más de {MaximoAsientos} asientos.", 400));
            }

            if (coordenadas.Count > 0 && coordenadas.Count > evento.Capacidad)
            {
                return Preparacion<T>.Fallida(
                    Error<T>($"No puede seleccionar más de {evento.Capacidad} asientos.", 400));
            }

            var total = 0;

            if (calcularTotal)
            {
                try
                {
                    total = checked(coordenadas.Count * evento.PrecioEntrada);
                }
                catch (OverflowException)
                {
                    return Preparacion<T>.Fallida(
                        Error<T>("El total de la selección supera el valor permitido.", 400));
                }
            }

            return Preparacion<T>.Valida(evento, coordenadas, total);
        }

        private static MapaAsientosDto CrearMapa(
            EventoDto evento,
            IReadOnlyCollection<Coordenada> seleccionadas,
            IReadOnlySet<Coordenada> ocupados)
        {
            var marcadas = seleccionadas.ToHashSet();
            var asientos = new List<AsientoDisponibleDto>(MaximoAsientos);

            for (var fila = 1; fila <= Filas; fila++)
            {
                for (var columna = 1; columna <= Columnas; columna++)
                {
                    var coordenada = new Coordenada(fila, columna);

                    asientos.Add(new AsientoDisponibleDto
                    {
                        Fila = fila,
                        Columna = columna,
                        Etiqueta = CrearEtiqueta(coordenada),
                        Seleccionado = marcadas.Contains(coordenada),
                        Ocupado = ocupados.Contains(coordenada)
                    });
                }
            }

            return new MapaAsientosDto
            {
                EventoId = evento.Id,
                Nombre = evento.Nombre,
                Capacidad = evento.Capacidad,
                PrecioEntrada = evento.PrecioEntrada,
                Asientos = asientos
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

        private static string CrearPosicion(Coordenada coordenada)
        {
            return $"{coordenada.Fila}-{coordenada.Columna}";
        }

        private static string CrearEtiqueta(Coordenada coordenada)
        {
            return $"{(char)('A' + coordenada.Fila - 1)}{coordenada.Columna}";
        }

        private static Respuesta<T> Exito<T>(T dato)
        {
            return new Respuesta<T>
            {
                Dato = dato
            };
        }

        private static Respuesta<T> Error<T>(string mensaje, int codigo)
        {
            return new Respuesta<T>
            {
                EsCorrecto = false,
                Mensaje = mensaje,
                Codigo = codigo
            };
        }

        private readonly record struct Coordenada(int Fila, int Columna);

        private sealed class Preparacion<T>
        {
            public EventoDto? Evento { get; private init; }

            public List<Coordenada>? Coordenadas { get; private init; }

            public int Total { get; private init; }

            public Respuesta<T>? Error { get; private init; }

            public static Preparacion<T> Valida(
                EventoDto evento,
                List<Coordenada> coordenadas,
                int total)
            {
                return new Preparacion<T>
                {
                    Evento = evento,
                    Coordenadas = coordenadas,
                    Total = total
                };
            }

            public static Preparacion<T> Fallida(Respuesta<T> error)
            {
                return new Preparacion<T>
                {
                    Error = error
                };
            }
        }
    }
}
