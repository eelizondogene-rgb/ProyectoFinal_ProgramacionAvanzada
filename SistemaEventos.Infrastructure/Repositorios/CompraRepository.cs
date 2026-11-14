using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SistemaEventos.Application.Interfaces;
using SistemaEventos.Domain.Entidades;
using SistemaEventos.Infrastructure.Datos;

namespace SistemaEventos.Infrastructure.Repositorios
{
    public class CompraRepository : ICompraRepository
    {
        private readonly ApplicationDbContext _context;

        public CompraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Asiento>> ObtenerAsientosVendidosAsync(
            int eventoId,
            CancellationToken cancellationToken = default)
        {
            if (eventoId <= 0)
            {
                return new List<Asiento>();
            }

            return await _context.Boletos
                .AsNoTracking()
                .Where(b => b.EventoId == eventoId)
                .Select(b => b.Asiento)
                .ToListAsync(cancellationToken);
        }

        public async Task<Compra?> GuardarCompraAsync(
            Compra compra,
            CancellationToken cancellationToken = default)
        {
            if (!PuedeGuardarse(compra))
            {
                return null;
            }

            await using var transaccion = await _context.Database
                .BeginTransactionAsync(cancellationToken);

            var anteriores = _context.ChangeTracker
                .Entries()
                .Select(entrada => entrada.Entity)
                .ToHashSet();

            try
            {
                var preparada = await PrepararBoletosAsync(compra, cancellationToken);

                if (!preparada)
                {
                    await RevertirAsync(transaccion, anteriores);
                    return null;
                }

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync(cancellationToken);
                await transaccion.CommitAsync(cancellationToken);
                return compra;
            }
            catch (OperationCanceledException)
            {
                await RevertirAsync(transaccion, anteriores);
                throw;
            }
            catch (DbUpdateException excepcion) when (EsConflictoDeIndiceUnico(excepcion))
            {
                await RevertirAsync(transaccion, anteriores);
                return null;
            }
            catch (Exception)
            {
                await RevertirAsync(transaccion, anteriores);
                throw;
            }
        }

        private static bool PuedeGuardarse(Compra compra)
        {
            if (compra is null
                || compra.Id != 0
                || compra.UsuarioId <= 0
                || compra.EventoId <= 0
                || compra.Boletos is null
                || compra.Boletos.Count == 0)
            {
                return false;
            }

            var posiciones = new HashSet<(int Fila, int Columna)>();

            foreach (var boleto in compra.Boletos)
            {
                if (boleto.Asiento is null
                    || boleto.Asiento.Fila < 1
                    || boleto.Asiento.Columna < 1)
                {
                    return false;
                }

                if (!posiciones.Add((boleto.Asiento.Fila, boleto.Asiento.Columna)))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> PrepararBoletosAsync(
            Compra compra,
            CancellationToken cancellationToken)
        {
            var asientosDelEvento = await _context.Asientos
                .Where(a => a.EventoId == compra.EventoId)
                .ToListAsync(cancellationToken);

            var asientosVendidos = await _context.Boletos
                .Where(b => b.EventoId == compra.EventoId)
                .Select(b => b.AsientoId)
                .ToListAsync(cancellationToken);

            var vendidos = asientosVendidos.ToHashSet();

            foreach (var boleto in compra.Boletos)
            {
                var fila = boleto.Asiento.Fila;
                var columna = boleto.Asiento.Columna;

                var asiento = asientosDelEvento.FirstOrDefault(a =>
                    a.Fila == fila && a.Columna == columna);

                if (asiento is null)
                {
                    asiento = new Asiento
                    {
                        EventoId = compra.EventoId,
                        Fila = fila,
                        Columna = columna
                    };

                    _context.Asientos.Add(asiento);
                    asientosDelEvento.Add(asiento);
                }
                else if (asiento.Id != 0 && vendidos.Contains(asiento.Id))
                {
                    return false;
                }

                boleto.Asiento = asiento;
                boleto.EventoId = compra.EventoId;
                boleto.Compra = compra;

                if (asiento.Id != 0)
                {
                    boleto.AsientoId = asiento.Id;
                }
            }

            return true;
        }

        private static bool EsConflictoDeIndiceUnico(DbUpdateException excepcion)
        {
            for (Exception? actual = excepcion; actual is not null; actual = actual.InnerException)
            {
                if (actual is SqlException sql
                    && (sql.Number == 2601 || sql.Number == 2627))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task RevertirAsync(
            IDbContextTransaction transaccion,
            HashSet<object> anteriores)
        {
            var incorporadas = _context.ChangeTracker
                .Entries()
                .Where(entrada => !anteriores.Contains(entrada.Entity))
                .ToList();

            foreach (var entrada in incorporadas)
            {
                entrada.State = EntityState.Detached;
            }

            try
            {
                await transaccion.RollbackAsync(CancellationToken.None);
            }
            catch (Exception)
            {
                // La transacción ya no admite reversión.
                // La excepción original sigue su curso en el método que llamó.
            }
        }
    }
}
