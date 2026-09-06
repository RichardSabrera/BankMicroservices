using CuentaMovimientoApi.Application.Services;
using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CuentaMovimientoApi.Controllers;

[ApiController]
[Route("movimientos")]
[Route("api/movimientos")]
[ManejoExcepciones]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoService _movimientoService;

    public MovimientosController(IMovimientoService movimientoService)
    {
        _movimientoService = movimientoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos()
    {
        var movimientos = await _movimientoService.GetAllAsync();
        return Ok(movimientos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Movimiento>> GetMovimiento(int id)
    {
        var movimiento = await _movimientoService.GetByIdAsync(id);
        if (movimiento == null)
        {
            return NotFound(new { mensaje = $"Movimiento con ID {id} no encontrado" });
        }
        return Ok(movimiento);
    }

    [HttpPost]
    public async Task<ActionResult<Movimiento>> CreateMovimiento([FromBody] Movimiento movimiento)
    {
        try
        {
            var created = await _movimientoService.CreateAsync(movimiento);
            return CreatedAtAction(nameof(GetMovimiento), new { id = created.Id }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovimiento(int id, [FromBody] Movimiento movimiento)
    {
        await _movimientoService.UpdateAsync(id, movimiento);
        return Ok(movimiento);
    }
}
