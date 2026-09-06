using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Exceptions;
using CuentaMovimientoApi.Filters;
using CuentaMovimientoApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuentaMovimientoApi.Controllers;

[ApiController]
[Route("movimientos")]
[Route("api/movimientos")]
[ManejoExcepciones]
public class MovimientosController : ControllerBase
{
    private readonly CuentaDbContext _context;

    public MovimientosController(CuentaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos()
    {
        return await _context.Movimientos.OrderByDescending(m => m.Fecha).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movimiento>> GetMovimiento(int id)
    {
        var movimiento = await _context.Movimientos.FindAsync(id);
        if (movimiento == null)
        {
            return NotFound(new { mensaje = $"Movimiento con ID {id} no encontrado" });
        }
        return movimiento;
    }

    [HttpPost]
    public async Task<ActionResult<Movimiento>> CreateMovimiento([FromBody] Movimiento movimiento)
    {
        var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == movimiento.NumeroCuenta);
        if (cuenta == null)
        {
            return NotFound(new { mensaje = $"La cuenta '{movimiento.NumeroCuenta}' no existe." });
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

        return CreatedAtAction(nameof(GetMovimiento), new { id = movimiento.Id }, movimiento);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovimiento(int id, [FromBody] Movimiento movimiento)
    {
        if (id != movimiento.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el objeto" });
        }

        _context.Entry(movimiento).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(movimiento);
    }
}
