using CuentaMovimientoApi.Domain.Entities;

namespace CuentaMovimientoApi.Application.Services;

public interface IMovimientoService
{
    Task<IEnumerable<Movimiento>> GetAllAsync();
    Task<Movimiento?> GetByIdAsync(int id);
    Task<Movimiento> CreateAsync(Movimiento movimiento);
    Task UpdateAsync(int id, Movimiento movimiento);
}
