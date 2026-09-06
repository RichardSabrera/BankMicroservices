-- =============================================
-- BASE DE DATOS 1: DevSuDBCliente (Microservicio ClientePersonaApi)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DevSuDBCliente')
BEGIN
    CREATE DATABASE DevSuDBCliente;
END
GO

USE DevSuDBCliente;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type in (N'U'))
BEGIN
    CREATE TABLE Clientes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Genero NVARCHAR(20) NOT NULL,
        Edad INT NOT NULL,
        Identificacion NVARCHAR(20) NOT NULL UNIQUE,
        Direccion NVARCHAR(200) NOT NULL,
        Telefono NVARCHAR(20) NOT NULL,
        ClienteId NVARCHAR(50) NOT NULL UNIQUE,
        Contrasena NVARCHAR(100) NOT NULL,
        Estado BIT NOT NULL DEFAULT 1
    );
END
GO

-- Insertar Datos Iniciales con Hash de ClienteId
IF NOT EXISTS (SELECT * FROM Clientes WHERE Identificacion = '1712345678')
BEGIN
    INSERT INTO Clientes (Nombre, Genero, Edad, Identificacion, Direccion, Telefono, ClienteId, Contrasena, Estado)
    VALUES 
    (N'Jose Lema', N'Masculino', 35, N'1712345678', N'Otavalo sn y principal', N'098254785', N'CLI-A8F93B12C4E5', N'1234', 1),
    (N'Marianela Montalvo', N'Femenino', 30, N'1798765432', N'Amazonas y NNUU', N'097548965', N'CLI-B9E02A14D5F6', N'5678', 1),
    (N'Juan Osorio', N'Masculino', 40, N'1755554444', N'13 junio y Equinoccial', N'098874587', N'CLI-C1D2E3F4A5B6', N'1245', 1);
END
GO


-- =============================================
-- BASE DE DATOS 2: DevSuDBMovimientos (Microservicio CuentaMovimientoApi)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DevSuDBMovimientos')
BEGIN
    CREATE DATABASE DevSuDBMovimientos;
END
GO

USE DevSuDBMovimientos;
GO

-- 1. TABLA ClientesReadModel (Sincronizada asíncronamente vía RabbitMQ)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ClientesReadModel]') AND type in (N'U'))
BEGIN
    CREATE TABLE ClientesReadModel (
        Id INT PRIMARY KEY,
        ClienteId NVARCHAR(50) NOT NULL UNIQUE,
        Nombre NVARCHAR(100) NOT NULL,
        Identificacion NVARCHAR(20) NOT NULL,
        Estado BIT NOT NULL DEFAULT 1
    );
END
GO

-- 2. TABLA CUENTAS (Relacionada a ClientesReadModel por FK entero)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cuentas]') AND type in (N'U'))
BEGIN
    CREATE TABLE Cuentas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NumeroCuenta NVARCHAR(20) NOT NULL UNIQUE,
        TipoCuenta NVARCHAR(20) NOT NULL,
        SaldoInicial DECIMAL(18,2) NOT NULL,
        SaldoDisponible DECIMAL(18,2) NOT NULL,
        Estado BIT NOT NULL DEFAULT 1,
        ClienteId INT NOT NULL,
        CONSTRAINT FK_Cuentas_ClientesReadModel FOREIGN KEY (ClienteId) REFERENCES ClientesReadModel(Id) ON DELETE CASCADE
    );
END
GO

-- 3. TABLA MOVIMIENTOS
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Movimientos]') AND type in (N'U'))
BEGIN
    CREATE TABLE Movimientos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        TipoMovimiento NVARCHAR(50) NOT NULL,
        Valor DECIMAL(18,2) NOT NULL,
        Saldo DECIMAL(18,2) NOT NULL,
        NumeroCuenta NVARCHAR(20) NOT NULL,
        CONSTRAINT FK_Movimientos_Cuentas FOREIGN KEY (NumeroCuenta) REFERENCES Cuentas(NumeroCuenta) ON DELETE CASCADE
    );
END
GO

-- Insertar Datos Iniciales Sincronizados en ClientesReadModel
IF NOT EXISTS (SELECT * FROM ClientesReadModel WHERE Id = 1)
BEGIN
    INSERT INTO ClientesReadModel (Id, ClienteId, Nombre, Identificacion, Estado)
    VALUES 
    (1, N'CLI-A8F93B12C4E5', N'Jose Lema', N'1712345678', 1),
    (2, N'CLI-B9E02A14D5F6', N'Marianela Montalvo', N'1798765432', 1),
    (3, N'CLI-C1D2E3F4A5B6', N'Juan Osorio', N'1755554444', 1);
END
GO

-- Insertar Cuentas de Usuario (ClienteId entero asociando a ClientesReadModel.Id)
IF NOT EXISTS (SELECT * FROM Cuentas WHERE NumeroCuenta = '478758')
BEGIN
    INSERT INTO Cuentas (NumeroCuenta, TipoCuenta, SaldoInicial, SaldoDisponible, Estado, ClienteId)
    VALUES 
    (N'478758', N'Ahorros', 2000.00, 1425.00, 1, 1),
    (N'225487', N'Corriente', 100.00, 700.00, 1, 2),
    (N'495878', N'Ahorros', 0.00, 150.00, 1, 3),
    (N'496825', N'Ahorros', 540.00, 0.00, 1, 2),
    (N'585545', N'Corriente', 1000.00, 1000.00, 1, 1);
END
GO

-- Insertar Movimientos Requeridos
IF NOT EXISTS (SELECT * FROM Movimientos WHERE NumeroCuenta = '478758')
BEGIN
    INSERT INTO Movimientos (NumeroCuenta, TipoMovimiento, Valor, Saldo, Fecha)
    VALUES 
    (N'478758', N'Retiro de 575', -575.00, 1425.00, '2022-02-08 00:00:00'),
    (N'225487', N'Deposito de 600', 600.00, 700.00, '2022-02-10 00:00:00'),
    (N'495878', N'Deposito de 150', 150.00, 150.00, '2022-02-08 00:00:00'),
    (N'496825', N'Retiro de 540', -540.00, 0.00, '2022-02-08 00:00:00');
END
GO
