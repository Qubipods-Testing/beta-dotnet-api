# Propuesta: Segmento del cliente en la etiqueta

Solicitud: REQ-2026-005 · Línea base: DEF-2026-005 v1.0.0 · Incremento: INCR-01

## Why

Los sistemas consumidores de `GET /api/clientes/{id}` hoy reciben solo los datos
básicos del cliente (`Id`, `NombreCompleto`, `FechaAlta`) y tienen que calcular por
su cuenta si el cliente es "Frecuente" o "Nuevo", duplicando la regla de negocio
BR-01 en cada consumidor. El DERCAS aprobado centraliza ese cálculo en
`beta-dotnet-api`, el dueño del dato, y lo publica como contrato versionado
(`clientes-api`) para que cualquier pod pueda consumirlo sin reimplementar la regla.

## What Changes

- `beta-dotnet-api` calcula el segmento del cliente (`Frecuente` o `Nuevo`) según
  BR-01 y lo agrega al cuerpo de la respuesta de `GET /api/clientes/{id}`.
- Se publica el contrato `clientes-api` (tipo `http-api`) en su versión inicial,
  documentando la operación existente más el campo nuevo.
- Sin cambios de interfaz gráfica (la solicitud no tiene pantallas).
- Sin dependencias externas nuevas en `beta-dotnet-api` (NFR-01): el cálculo del
  segmento es lógica pura en el propio proyecto.

## Capabilities

### ADDED: `clientes-api` segmento de cliente
`GET /api/clientes/{id}` informa el segmento del cliente junto con sus datos
existentes, calculado en el servidor según BR-01.

## Acceptance Criteria (trazadas al DERCAS)

| AC | Escenario | Fuente |
|----|-----------|--------|
| AC-01.01.1 | Cliente con `FechaAlta` de hace un año → `Segmento: "Frecuente"` | US-01.01 |
| AC-01.01.2 | Cliente con `FechaAlta` de hace un mes → `Segmento: "Nuevo"` | US-01.01 |
| AC-01.01.3 | Id inexistente → 404 con `ErrorDto(Codigo: "CLIENTE_NO_ENCONTRADO")` | US-01.01 |

Regla de negocio aplicada: BR-01 — Frecuente si `FechaAlta` tiene 6 meses o más
respecto de hoy (UTC); si no, Nuevo.

## Impact

Una sola serie de construcción (Serie A): el cambio toca un endpoint existente, es
pequeño y autocontenido (DTO + regla + pruebas + contrato). Ver `tasks.md`.

Afectados:
- `src/BetaDotnetApi/Clientes.cs` (DTO y regla de segmento)
- `src/BetaDotnetApi/Program.cs` (uso del DTO ampliado)
- `tests/BetaDotnetApi.Tests/` (pruebas nuevas)
- `tests/BetaDotnetApi.Tests/BetaDotnetApi.Tests.csproj` (paquete de prueba para
  reemplazar SQL Server por un proveedor en memoria solo en pruebas; no afecta a
  `BetaDotnetApi.csproj`, ver NFR-01 en `design.md`)
- Contrato nuevo `clientes-api` (`contracts/clientes-api.contract.json`)

## Non-goals

- No se agregan nuevos endpoints ni se modifica la ruta de `GET /api/clientes/{id}`.
- No se cambia la fuente de datos (`ClientesDb`) ni su proveedor en producción
  (`SqlServer`).
- No hay interfaz gráfica que diseñar o construir.
- No se definen todavía consumidores del contrato `clientes-api` (quedan para
  solicitudes futuras).
