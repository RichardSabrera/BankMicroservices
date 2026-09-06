using CuentaMovimientoApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CuentaMovimientoApi.Infrastructure.Persistence;

public class CuentaDbContext : DbContext
{
    public CuentaDbContext(DbContextOptions<CuentaDbContext> options) : base(options)
    {
    }

    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Movimiento> Movimientos { get; set; }
    public DbSet<ClienteReadModel> Clientes { get; set; }
    public DbSet<ClienteReadModel> ClientesReadModel => Clientes;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClienteReadModel>(entity =>
        {
            entity.ToTable("ClientesReadModel");
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.ClienteId).IsUnique();
        });

        modelBuilder.Entity<Cuenta>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.NumeroCuenta).IsUnique();
            entity.HasOne<ClienteReadModel>()
                  .WithMany()
                  .HasForeignKey(c => c.ClienteId)
                  .HasPrincipalKey(cl => cl.Id)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(m => m.Id);
        });
    }
}
