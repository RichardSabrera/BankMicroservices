using CuentaMovimientoApi.DTOs;

namespace CuentaMovimientoApi.Application.Services;

public interface IReporteService
{
    Task<IEnumerable<ReporteDto>> GenerarReporteAsync(
        string? fecha,
        string? cliente,
        string? clienteId,
        DateTime? fechaInicio,
        DateTime? fechaFin);
}
