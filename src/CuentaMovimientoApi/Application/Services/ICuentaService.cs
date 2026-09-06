using CuentaMovimientoApi.Domain.Entities;

namespace CuentaMovimientoApi.Application.Services;

public interface ICuentaService
{
    Task<IEnumerable<Cuenta>> GetAllAsync();
    Task<Cuenta?> GetByIdAsync(int id);
    Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);
    Task<Cuenta> CreateAsync(Cuenta cuenta);
    Task UpdateAsync(int id, Cuenta cuenta);
    Task DeleteAsync(int id);
}
