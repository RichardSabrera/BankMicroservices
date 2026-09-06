using System.Text.Json;
using ClientePersonaApi.Domain.Entities;

namespace ClientePersonaApi.Application.Services;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByClienteIdAsync(string clienteId);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task UpdateAsync(int id, Cliente cliente);
    Task PatchAsync(int id, JsonElement patchData);
    Task DeleteAsync(int id);
}
