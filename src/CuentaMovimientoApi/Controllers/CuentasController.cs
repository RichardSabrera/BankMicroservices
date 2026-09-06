using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Filters;
using CuentaMovimientoApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuentaMovimientoApi.Controllers;

[ApiController]
[Route("cuentas")]
[Route("api/cuentas")]
[ManejoExcepciones]
public class CuentasController : ControllerBase
{
    private readonly CuentaDbContext _context;

    public CuentasController(CuentaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas()
    {
        return await _context.Cuentas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cuenta>> GetCuenta(int id)
    {
        var cuenta = await _context.Cuentas.FindAsync(id);
        if (cuenta == null)
        {
            return NotFound(new { mensaje = $"Cuenta con ID {id} no encontrada" });
        }
        return cuenta;
    }

    [HttpGet("numero/{numeroCuenta}")]
    public async Task<ActionResult<Cuenta>> GetByNumero(string numeroCuenta)
    {
        var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
        if (cuenta == null)
        {
            return NotFound(new { mensaje = $"Cuenta número {numeroCuenta} no encontrada" });
        }
        return cuenta;
    }

    [HttpPost]
    public async Task<ActionResult<Cuenta>> CreateCuenta([FromBody] Cuenta cuenta)
    {
        if (await _context.Cuentas.AnyAsync(c => c.NumeroCuenta == cuenta.NumeroCuenta))
        {
            throw new InvalidOperationException($"El número de cuenta '{cuenta.NumeroCuenta}' ya existe.");
        }

        cuenta.SaldoDisponible = cuenta.SaldoInicial;
        _context.Cuentas.Add(cuenta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCuenta), new { id = cuenta.Id }, cuenta);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCuenta(int id, [FromBody] Cuenta cuenta)
    {
        if (id != cuenta.Id)
        {
            return BadRequest(new { mensaje = "El ID de la ruta no coincide con el objeto" });
        }

        _context.Entry(cuenta).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(cuenta);
    }
}
