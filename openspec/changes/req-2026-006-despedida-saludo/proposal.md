# Propuesta: Despedida personalizada en el saludo

Solicitud: REQ-2026-006 · Línea base: DEF-2026-006 v1.0.0 · Incremento: INCR-01

## Why

El DERCAS aprobado pide que `beta-dotnet-api` ofrezca una despedida personalizada
para un nombre dado, análoga en espíritu al saludo que ya calcula el servicio para
sus clientes: un endpoint simple, sin estado y sin dependencias nuevas, que
cualquier consumidor pueda invocar pasando un nombre y recibir de vuelta el texto
de despedida ya formado, en vez de construir el literal "Hasta pronto, " en cada
consumidor.

## What Changes

- `beta-dotnet-api` agrega `GET /despedida` que responde `Hasta pronto, <nombre>`
  a partir de un nombre recibido por query string.
- Sin cambios de interfaz gráfica (la solicitud no tiene pantallas, ver DERCAS).
- Sin dependencias externas nuevas (el cálculo es concatenación de texto pura).
- Sin contratos nuevos: la solicitud no declara contratos a publicar ni a
  consumir.

## Capabilities

### ADDED: `despedida-api`
`GET /despedida` responde con un saludo de despedida personalizado para el
nombre recibido.

## Acceptance Criteria (trazadas al DERCAS)

| AC | Escenario | Fuente |
|----|-----------|--------|
| AC-01.01.1 | `GET /despedida?nombre=Ana` → `200 OK` con cuerpo `Hasta pronto, Ana` | US-01.01 |
| AC-01.01.2 | `GET /despedida` sin `nombre` → `400 Bad Request` con `ErrorDto(Codigo: "NOMBRE_REQUERIDO")` | US-01.01 (caso borde, ver `design.md`) |

## Impact

Una sola serie de construcción (Serie A): el cambio agrega un endpoint nuevo,
autocontenido y sin dependencias de datos. Ver `tasks.md`.

Afectados:
- `src/BetaDotnetApi/Despedida.cs` (función pura de formateo y endpoint)
- `src/BetaDotnetApi/Program.cs` (registro del endpoint nuevo)
- `tests/BetaDotnetApi.Tests/` (pruebas nuevas)
- `README.md` (documentar el endpoint nuevo)

## Non-goals

- No se modifica `GET /api/clientes/{id}` ni ningún otro endpoint existente.
- No se persiste ni se valida el nombre contra ninguna fuente de datos; es un
  endpoint sin estado.
- No hay interfaz gráfica que diseñar o construir.
- No se definen contratos de integración para este endpoint (la solicitud no lo
  pide).
