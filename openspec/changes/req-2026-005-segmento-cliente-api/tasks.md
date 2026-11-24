# Tareas — req-2026-005-segmento-cliente-api

Una sola serie: el cambio es pequeño (un DTO, una función pura, un endpoint
existente y sus pruebas) y no hay límite de tamaño de PR declarado en el repo.

## Serie A — Segmento del cliente y contrato clientes-api

1. [x] Rojo: agregar `SegmentadorTests` (unitarias) en
   `tests/BetaDotnetApi.Tests/` cubriendo BR-01: `FechaAlta` de hace un año →
   `Frecuente`, de hace un mes → `Nuevo`. Deben fallar porque `Segmentador` no
   existe todavía.
2. [x] Rojo: agregar `ClientesEndpointTests` (integración, `WebApplicationFactory`)
   con una fábrica de pruebas que reemplaza `UseSqlServer` por
   `UseInMemoryDatabase` solo en el proyecto de pruebas, cubriendo AC-01.01.1,
   AC-01.01.2 y AC-01.01.3 (incluye verificar `Codigo: "CLIENTE_NO_ENCONTRADO"`
   en el 404). Deben fallar porque la respuesta todavía no trae `Segmento`.
3. [x] Agregar el paquete `Microsoft.EntityFrameworkCore.InMemory` (versión
   7.0.x, acorde al resto del proyecto) solo a
   `tests/BetaDotnetApi.Tests/BetaDotnetApi.Tests.csproj`. No tocar
   `src/BetaDotnetApi/BetaDotnetApi.csproj` (NFR-01).
4. [x] Verde: implementar `Segmentador.Calcular` en
   `src/BetaDotnetApi/Clientes.cs` (ver `design.md`, ADR-01).
5. [x] Verde: ampliar `ClienteDto` con `Segmento` y actualizar
   `ClienteDto.Desde` para recibir el instante UTC actual; actualizar
   `src/BetaDotnetApi/Program.cs` para pasar `DateTime.UtcNow` al construir la
   respuesta de `GET /api/clientes/{id}`.
6. [x] Confirmar verde: `dotnet test` desde la raíz del repo (todas las pruebas,
   incluida `FormatoTests` existente).
7. [x] Escribir `openspec/changes/req-2026-005-segmento-cliente-api/contracts/clientes-api.contract.json`
   (ver `design.md`, sección `## Contratos`, para el casing y las decisiones).
8. [x] Validar el contrato:
   `node .claude/skills/generate-spec/check-contracts.mjs openspec/changes/req-2026-005-segmento-cliente-api/contracts/*.contract.json`
   — corregir cualquier `ERROR` y repetir hasta que pase.
9. [x] `npx openspec validate req-2026-005-segmento-cliente-api --strict` — corregir
   hasta que pase.
10. [x] Actualizar `README.md` del repo para mencionar el campo `Segmento` en la
    descripción de `GET /api/clientes/{id}`.
