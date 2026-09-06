using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Infrastructure.Persistence;

namespace CuentaMovimientoApi.Data;

public static class CuentaSeedData
{
    public static void Seed(CuentaDbContext context)
    {
        if (!context.Clientes.Any())
        {
            context.Clientes.AddRange(
                new ClienteReadModel { Id = 1, ClienteId = "CLI-A8F93B12C4E5", Nombre = "Jose Lema", Identificacion = "1712345678", Estado = true },
                new ClienteReadModel { Id = 2, ClienteId = "CLI-B9E02A14D5F6", Nombre = "Marianela Montalvo", Identificacion = "1798765432", Estado = true },
                new ClienteReadModel { Id = 3, ClienteId = "CLI-C1D2E3F4A5B6", Nombre = "Juan Osorio", Identificacion = "1755554444", Estado = true }
            );
            context.SaveChanges();
        }

        if (!context.Cuentas.Any())
        {
            var cuenta1 = new Cuenta { NumeroCuenta = "478758", TipoCuenta = "Ahorros", SaldoInicial = 2000, SaldoDisponible = 1425, Estado = true, ClienteId = 1 };
            var cuenta2 = new Cuenta { NumeroCuenta = "225487", TipoCuenta = "Corriente", SaldoInicial = 100, SaldoDisponible = 700, Estado = true, ClienteId = 2 };
            var cuenta3 = new Cuenta { NumeroCuenta = "495878", TipoCuenta = "Ahorros", SaldoInicial = 0, SaldoDisponible = 150, Estado = true, ClienteId = 3 };
            var cuenta4 = new Cuenta { NumeroCuenta = "496825", TipoCuenta = "Ahorros", SaldoInicial = 540, SaldoDisponible = 0, Estado = true, ClienteId = 2 };
            var cuenta5 = new Cuenta { NumeroCuenta = "585545", TipoCuenta = "Corriente", SaldoInicial = 1000, SaldoDisponible = 1000, Estado = true, ClienteId = 1 };

            context.Cuentas.AddRange(cuenta1, cuenta2, cuenta3, cuenta4, cuenta5);
            context.SaveChanges();

            context.Movimientos.AddRange(
                new Movimiento
                {
                    NumeroCuenta = "478758",
                    TipoMovimiento = "Retiro de 575",
                    Valor = -575,
                    Saldo = 1425,
                    Fecha = new DateTime(2022, 2, 8)
                },
                new Movimiento
                {
                    NumeroCuenta = "225487",
                    TipoMovimiento = "Deposito de 600",
                    Valor = 600,
                    Saldo = 700,
                    Fecha = new DateTime(2022, 2, 10)
                },
                new Movimiento
                {
                    NumeroCuenta = "495878",
                    TipoMovimiento = "Deposito de 150",
                    Valor = 150,
                    Saldo = 150,
                    Fecha = new DateTime(2022, 2, 8)
                },
                new Movimiento
                {
                    NumeroCuenta = "496825",
                    TipoMovimiento = "Retiro de 540",
                    Valor = -540,
                    Saldo = 0,
                    Fecha = new DateTime(2022, 2, 8)
                }
            );

            context.SaveChanges();
        }
    }
}
