using CuentaMovimientoApi.Application.Services;
using CuentaMovimientoApi.DTOs;
using CuentaMovimientoApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CuentaMovimientoApi.Controllers;

[ApiController]
[Route("reportes")]
[Route("api/reportes")]
[ManejoExcepciones]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReporteDto>>> GetReporte(
        [FromQuery] string? fecha,
        [FromQuery] string? cliente,
        [FromQuery] string? clienteId,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin)
    {
        try
        {
            var resultado = await _reporteService.GenerarReporteAsync(fecha, cliente, clienteId, fechaInicio, fechaFin);
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
