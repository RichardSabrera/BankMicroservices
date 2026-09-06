# Sistema de Microservicios Bancarios - Devsu Technical Challenge (.NET 8 + Clean Architecture)

Solución construida con **.NET 8 (C# 12)** bajo los principios de **Clean Architecture** (Domain, Application, Infrastructure, Presentation), **Entity Framework Core 8**, **RabbitMQ**, bases de datos desacopladas en **Microsoft SQL Server** (`DevSuDBCliente` y `DevSuDBMovimientos`) y contenedorización con **Docker Compose**.

---

## 🏗️ Estructura del Proyecto (Clean Architecture)

Cada microservicio se encuentra estructurado en capas desacopladas:

```text
src/
├── ClientePersonaApi/
│   ├── Domain/                 # Entidades e Interfaz del Repositorio (Persona, Cliente, IClienteRepository)
│   ├── Application/            # Casos de Uso y Servicios de Aplicación (IClienteService, ClienteService)
│   ├── Infrastructure/         # Persistencia y Mensajería (ClienteDbContext, ClienteRepository, RabbitMqPublisher)
│   └── Controllers/            # API Rest y Filtro de Excepciones (ClientesController, ManejoExcepcionesFilter)
│
└── CuentaMovimientoApi/
    ├── Domain/                 # Entidades y Excepciones de Dominio (Cuenta, Movimiento, ClienteReadModel, SaldoNoDisponibleException)
    ├── Application/            # DTOs y Lógica de Reportes y Transacciones (ReporteDto, Services)
    ├── Infrastructure/         # Persistencia y Consumidor RabbitMQ (CuentaDbContext, RabbitMqConsumer)
    └── Controllers/            # API Rest (CuentasController, MovimientosController, ReportesController)
```

---

## 🚀 Arquitectura de Microservicios y Bases de Datos Desacopladas

1. **`ClientePersonaApi` (Puerto 8081 | Base de datos: `DevSuDBCliente`):**
   * Gestiona el CRUD de Clientes/Personas.
   * Autogenera hashes únicos SHA-256 (`CLI-XXXXXX`) en el campo `ClienteId` si no es provisto.
   * Publica eventos de creación/actualización en RabbitMQ (`cliente_eventos`).

2. **`CuentaMovimientoApi` (Puerto 8082 | Base de datos: `DevSuDBMovimientos`):**
   * Gestiona Cuentas, Movimientos y Reportes.
   * Mantiene su tabla local `ClientesReadModel` sincronizada asíncronamente vía RabbitMQ.
   * Define la clave foránea entera `ClienteId (INT)` relacionando Cuentas hacia `ClientesReadModel(Id)`.
   * Atributo decorador `[ManejoExcepciones]` que retorna `"Saldo no disponible"` cuando un retiro supera el saldo disponible.

---

## 📦 Despliegue en Contenedores (Docker Compose)

```bash
# 1. Posicionarse en la carpeta raíz
cd C:\Users\Rsabr\OneDrive\Escritorio\ProjectDevsu

# 2. Levantar la infraestructura completa (2 BD SQL Server + RabbitMQ + 2 Microservicios)
docker-compose up --build -d
```

URLs disponibles:
* **ClientePersonaApi Swagger:** `http://localhost:8081/swagger`
* **CuentaMovimientoApi Swagger:** `http://localhost:8082/swagger`
* **RabbitMQ Manager:** `http://localhost:15672` (guest/guest)

---

## 🧪 Pruebas Automatizadas (F5 y F6)

```bash
dotnet test BankMicroservices.slnx
```

---

## 📄 Archivos Entregables

* Script de Base de Datos: [`BaseDatos.sql`](file:///C:/Users/Rsabr/OneDrive/Escritorio/ProjectDevsu/BaseDatos.sql)
* Colección Postman: [`Postman_Collection.json`](file:///C:/Users/Rsabr/OneDrive/Escritorio/ProjectDevsu/Postman_Collection.json)
* Orquestación Docker: [`docker-compose.yml`](file:///C:/Users/Rsabr/OneDrive/Escritorio/ProjectDevsu/docker-compose.yml)
