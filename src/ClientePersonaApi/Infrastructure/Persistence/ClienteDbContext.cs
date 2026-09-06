using ClientePersonaApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientePersonaApi.Infrastructure.Persistence;

public class ClienteDbContext : DbContext
{
    public ClienteDbContext(DbContextOptions<ClienteDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.ClienteId).IsUnique();
            entity.HasIndex(c => c.Identificacion).IsUnique();
        });
    }
}
