using ClientePersonaApi.Domain.Entities;
using ClientePersonaApi.Domain.Interfaces;
using ClientePersonaApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClientePersonaApi.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly ClienteDbContext _context;

    public ClienteRepository(ClienteDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<Cliente?> GetByClienteIdAsync(string clienteId)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteId == clienteId);
    }

    public async Task<bool> ExistsByIdentificacionAsync(string identificacion)
    {
        return await _context.Clientes.AnyAsync(c => c.Identificacion == identificacion);
    }

    public async Task<bool> ExistsByClienteIdAsync(string clienteId)
    {
        return await _context.Clientes.AnyAsync(c => c.ClienteId == clienteId);
    }

    public async Task<Cliente> AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Entry(cliente).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Cliente cliente)
    {
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
    }
}
