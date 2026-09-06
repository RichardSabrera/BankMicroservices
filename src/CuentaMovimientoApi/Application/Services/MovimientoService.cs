using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Exceptions;
using CuentaMovimientoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CuentaMovimientoApi.Application.Services;

public class MovimientoService : IMovimientoService
{
    private readonly CuentaDbContext _context;

    public MovimientoService(CuentaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movimiento>> GetAllAsync()
    {
        return await _context.Movimientos.OrderByDescending(m => m.Fecha).ToListAsync();
    }

    public async Task<Movimiento?> GetByIdAsync(int id)
    {
        return await _context.Movimientos.FindAsync(id);
    }

    public async Task<Movimiento> CreateAsync(Movimiento movimiento)
    {
        var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == movimiento.NumeroCuenta);
        if (cuenta == null)
        {
            throw new KeyNotFoundException($"La cuenta '{movimiento.NumeroCuenta}' no existe.");
        }

        if (!string.IsNullOrEmpty(movimiento.TipoMovimiento) &&
            movimiento.TipoMovimiento.StartsWith("Retiro", StringComparison.OrdinalIgnoreCase) &&
            movimiento.Valor > 0)
        {
            movimiento.Valor = -movimiento.Valor;
        }

        decimal nuevoSaldo = cuenta.SaldoDisponible + movimiento.Valor;

        if (nuevoSaldo < 0)
        {
            throw new SaldoNoDisponibleException("Saldo no disponible");
        }

        cuenta.SaldoDisponible = nuevoSaldo;
        movimiento.Saldo = nuevoSaldo;

        if (movimiento.Fecha == default)
        {
            movimiento.Fecha = DateTime.Now;
        }

        _context.Movimientos.Add(movimiento);
        await _context.SaveChangesAsync();

        return movimiento;
    }

    public async Task UpdateAsync(int id, Movimiento movimiento)
    {
        if (id != movimiento.Id)
        {
            throw new ArgumentException("El ID de la ruta no coincide con el objeto.");
        }

        var existing = await _context.Movimientos.FindAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Movimiento con ID {id} no encontrado.");
        }

        _context.Entry(existing).CurrentValues.SetValues(movimiento);
        await _context.SaveChangesAsync();
    }
}
