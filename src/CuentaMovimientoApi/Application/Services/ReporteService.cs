using System.Globalization;
using CuentaMovimientoApi.DTOs;
using CuentaMovimientoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CuentaMovimientoApi.Application.Services;

public class ReporteService : IReporteService
{
    private readonly CuentaDbContext _context;

    public ReporteService(CuentaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReporteDto>> GenerarReporteAsync(
        string? fecha,
        string? cliente,
        string? clienteId,
        DateTime? fechaInicio,
        DateTime? fechaFin)
    {
        var targetCliente = cliente ?? clienteId;
        if (string.IsNullOrEmpty(targetCliente))
        {
            throw new ArgumentException("El parámetro 'cliente' o 'clienteId' es requerido.");
        }

        DateTime inicio = DateTime.MinValue;
        DateTime fin = DateTime.MaxValue;

        if (!string.IsNullOrEmpty(fecha))
        {
            var partes = fecha.Split(',');
            if (partes.Length >= 2)
            {
                DateTime.TryParse(partes[0].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out inicio);
                DateTime.TryParse(partes[1].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out fin);
            }
        }
        else
        {
            if (fechaInicio.HasValue) inicio = fechaInicio.Value;
            if (fechaFin.HasValue) fin = fechaFin.Value;
        }

        int parsedId = 0;
        bool isIdParsed = int.TryParse(targetCliente, out parsedId);

        var clienteInfo = await _context.Clientes
            .FirstOrDefaultAsync(c => (isIdParsed && c.Id == parsedId) || c.ClienteId == targetCliente || c.Nombre.ToLower() == targetCliente.ToLower());

        if (clienteInfo == null)
        {
            return new List<ReporteDto>();
        }

        int searchId = clienteInfo.Id;
        string nombreCliente = clienteInfo.Nombre;

        var cuentas = await _context.Cuentas
            .Where(c => c.ClienteId == searchId)
            .ToListAsync();

        if (!cuentas.Any())
        {
            return new List<ReporteDto>();
        }

        var numerosCuenta = cuentas.Select(c => c.NumeroCuenta).ToList();

        var movimientos = await _context.Movimientos
            .Where(m => numerosCuenta.Contains(m.NumeroCuenta) && m.Fecha >= inicio && m.Fecha <= fin)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();

        var resultado = new List<ReporteDto>();

        foreach (var mov in movimientos)
        {
            var cuenta = cuentas.First(c => c.NumeroCuenta == mov.NumeroCuenta);
            decimal saldoInicialMov = mov.Saldo - mov.Valor;

            resultado.Add(new ReporteDto
            {
                Fecha = mov.Fecha.ToString("d/M/yyyy"),
                Cliente = nombreCliente,
                NumeroCuenta = cuenta.NumeroCuenta,
                Tipo = cuenta.TipoCuenta,
                SaldoInicial = saldoInicialMov,
                Estado = cuenta.Estado,
                Movimiento = mov.Valor,
                SaldoDisponible = mov.Saldo
            });
        }

        return resultado;
    }
}
