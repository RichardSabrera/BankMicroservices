using CuentaMovimientoApi.Application.Services;
using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CuentaMovimientoApi.Controllers;

[ApiController]
[Route("cuentas")]
[Route("api/cuentas")]
[ManejoExcepciones]
public class CuentasController : ControllerBase
{
    private readonly ICuentaService _cuentaService;

    public CuentasController(ICuentaService cuentaService)
    {
        _cuentaService = cuentaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas()
    {
        var cuentas = await _cuentaService.GetAllAsync();
        return Ok(cuentas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cuenta>> GetCuenta(int id)
    {
        var cuenta = await _cuentaService.GetByIdAsync(id);
        if (cuenta == null)
        {
            return NotFound(new { mensaje = $"Cuenta con ID {id} no encontrada" });
        }
        return Ok(cuenta);
    }

    [HttpGet("numero/{numeroCuenta}")]
    public async Task<ActionResult<Cuenta>> GetByNumero(string numeroCuenta)
    {
        var cuenta = await _cuentaService.GetByNumeroCuentaAsync(numeroCuenta);
        if (cuenta == null)
        {
            return NotFound(new { mensaje = $"Cuenta número {numeroCuenta} no encontrada" });
        }
        return Ok(cuenta);
    }

    [HttpPost]
    public async Task<ActionResult<Cuenta>> CreateCuenta([FromBody] Cuenta cuenta)
    {
        var created = await _cuentaService.CreateAsync(cuenta);
        return CreatedAtAction(nameof(GetCuenta), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCuenta(int id, [FromBody] Cuenta cuenta)
    {
        await _cuentaService.UpdateAsync(id, cuenta);
        return Ok(cuenta);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCuenta(int id)
    {
        await _cuentaService.DeleteAsync(id);
        return Ok(new { mensaje = "Cuenta eliminada correctamente" });
    }
}
