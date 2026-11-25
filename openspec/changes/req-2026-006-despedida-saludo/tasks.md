# Tareas — req-2026-006-despedida-saludo

Una sola serie: el cambio es pequeño (un endpoint nuevo sin estado, una función
pura y sus pruebas) y no hay límite de tamaño de PR declarado en el repo.

## Serie A — Endpoint GET /despedida

1. [ ] Rojo: agregar `DespedidasTests` (unitarias) en
   `tests/BetaDotnetApi.Tests/` cubriendo AC-01.01.1: `Despedidas.Formatear("Ana")`
   devuelve `"Hasta pronto, Ana"`. Debe fallar porque `Despedidas` no existe
   todavía.
2. [ ] Rojo: agregar `DespedidaEndpointTests` (integración, `WebApplicationFactory`)
   con una fábrica de pruebas que reemplaza `UseSqlServer` por
   `UseInMemoryDatabase` solo en el proyecto de pruebas, cubriendo AC-01.01.1
   (`GET /despedida?nombre=Ana` → `200`, cuerpo `Hasta pronto, Ana`) y AC-01.01.2
   (`GET /despedida` sin `nombre` → `400` con `Codigo: "NOMBRE_REQUERIDO"`). Deben
   fallar porque el endpoint no existe todavía.
3. [ ] Agregar el paquete `Microsoft.EntityFrameworkCore.InMemory` (versión
   7.0.x, acorde al resto del proyecto) solo a
   `tests/BetaDotnetApi.Tests/BetaDotnetApi.Tests.csproj`. No tocar
   `src/BetaDotnetApi/BetaDotnetApi.csproj`.
4. [ ] Verde: crear `src/BetaDotnetApi/Despedida.cs` con
   `Despedidas.Formatear(string nombre)` (ver `design.md`, ADR-01).
5. [ ] Verde: registrar `MapGet("/despedida", ...)` en
   `src/BetaDotnetApi/Program.cs`: responde `400` con
   `ErrorDto("NOMBRE_REQUERIDO", ...)` cuando `nombre` es nulo o vacío, y
   `200` con `Results.Text(Despedidas.Formatear(nombre))` en caso contrario (ver
   `design.md`, ADR-02).
6. [ ] Confirmar verde: `dotnet test` desde la raíz del repo (todas las pruebas,
   incluidas las existentes).
7. [ ] `npx openspec validate req-2026-006-despedida-saludo --strict` — corregir
   hasta que pase.
8. [ ] Actualizar `README.md` del repo para documentar `GET /despedida`.
