using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ClientePersonaApi.Domain.Entities;
using ClientePersonaApi.Domain.Interfaces;
using ClientePersonaApi.Services;

namespace ClientePersonaApi.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IRabbitMqPublisher _publisher;

    public ClienteService(IClienteRepository repository, IRabbitMqPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Cliente?> GetByClienteIdAsync(string clienteId)
    {
        return await _repository.GetByClienteIdAsync(clienteId);
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        if (await _repository.ExistsByIdentificacionAsync(cliente.Identificacion))
        {
            throw new InvalidOperationException($"La identificación '{cliente.Identificacion}' ya se encuentra registrada.");
        }

        if (string.IsNullOrWhiteSpace(cliente.ClienteId))
        {
            using var sha256 = SHA256.Create();
            var rawInput = $"{cliente.Identificacion}_{cliente.Nombre}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawInput));
            var hashHex = Convert.ToHexString(hashBytes)[..12];
            cliente.ClienteId = $"CLI-{hashHex}";
        }

        if (await _repository.ExistsByClienteIdAsync(cliente.ClienteId))
        {
            cliente.ClienteId = $"CLI-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
        }

        var created = await _repository.AddAsync(cliente);

        _publisher.PublishClientCreated(new
        {
            created.Id,
            created.ClienteId,
            created.Nombre,
            created.Identificacion,
            created.Estado
        });

        return created;
    }

    public async Task UpdateAsync(int id, Cliente cliente)
    {
        if (id != cliente.Id)
        {
            throw new ArgumentException("El ID de la ruta no coincide con el objeto.");
        }

        await _repository.UpdateAsync(cliente);

        _publisher.PublishClientCreated(new
        {
            cliente.Id,
            cliente.ClienteId,
            cliente.Nombre,
            cliente.Identificacion,
            cliente.Estado
        });
    }

    public async Task PatchAsync(int id, JsonElement patchData)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado.");
        }

        if (patchData.TryGetProperty("nombre", out var nombre)) cliente.Nombre = nombre.GetString()!;
        if (patchData.TryGetProperty("direccion", out var dir)) cliente.Direccion = dir.GetString()!;
        if (patchData.TryGetProperty("telefono", out var tel)) cliente.Telefono = tel.GetString()!;
        if (patchData.TryGetProperty("contrasena", out var pass)) cliente.Contrasena = pass.GetString()!;
        if (patchData.TryGetProperty("estado", out var st)) cliente.Estado = st.GetBoolean();

        await _repository.UpdateAsync(cliente);

        _publisher.PublishClientCreated(new
        {
            cliente.Id,
            cliente.ClienteId,
            cliente.Nombre,
            cliente.Identificacion,
            cliente.Estado
        });
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _repository.GetByIdAsync(id);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {id} no encontrado.");
        }

        await _repository.DeleteAsync(cliente);
    }
}
