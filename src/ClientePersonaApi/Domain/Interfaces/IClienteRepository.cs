using ClientePersonaApi.Domain.Entities;

namespace ClientePersonaApi.Domain.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByClienteIdAsync(string clienteId);
    Task<bool> ExistsByIdentificacionAsync(string identificacion);
    Task<bool> ExistsByClienteIdAsync(string clienteId);
    Task<Cliente> AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(Cliente cliente);
}
