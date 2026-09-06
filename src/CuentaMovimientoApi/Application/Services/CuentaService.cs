using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CuentaMovimientoApi.Application.Services;

public class CuentaService : ICuentaService
{
    private readonly CuentaDbContext _context;

    public CuentaService(CuentaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cuenta>> GetAllAsync()
    {
        return await _context.Cuentas.ToListAsync();
    }

    public async Task<Cuenta?> GetByIdAsync(int id)
    {
        return await _context.Cuentas.FindAsync(id);
    }

    public async Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta)
    {
        return await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
    }

    public async Task<Cuenta> CreateAsync(Cuenta cuenta)
    {
        if (await _context.Cuentas.AnyAsync(c => c.NumeroCuenta == cuenta.NumeroCuenta))
        {
            throw new InvalidOperationException($"El número de cuenta '{cuenta.NumeroCuenta}' ya existe.");
        }

        cuenta.SaldoDisponible = cuenta.SaldoInicial;
        _context.Cuentas.Add(cuenta);
        await _context.SaveChangesAsync();
        return cuenta;
    }

    public async Task UpdateAsync(int id, Cuenta cuenta)
    {
        if (id != cuenta.Id)
        {
            throw new ArgumentException("El ID de la ruta no coincide con el objeto.");
        }

        var existing = await _context.Cuentas.FindAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Cuenta con ID {id} no encontrada.");
        }

        _context.Entry(existing).CurrentValues.SetValues(cuenta);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cuenta = await _context.Cuentas.FindAsync(id);
        if (cuenta != null)
        {
            _context.Cuentas.Remove(cuenta);
            await _context.SaveChangesAsync();
        }
    }
}
