# Diseño: Segmento del cliente en la etiqueta

## Decisiones (DERCAS aprobado)

### ADR-01 · El segmento se calcula en el proveedor
> La regla vive en un solo lugar; los consumidores solo la muestran.

Se sigue tal cual: `beta-dotnet-api` calcula `Segmento` dentro de `GET
/api/clientes/{id}` antes de serializar la respuesta. Ningún consumidor recibe
`FechaAlta` sin el segmento ya resuelto.

Implementación: una función pura `Segmentador.Calcular(fechaAlta, ahoraUtc)` en
`src/BetaDotnetApi/Clientes.cs`, invocada desde el endpoint con `DateTime.UtcNow`
(BR-01 pide la comparación respecto de "hoy UTC"). `ClienteDto.Desde` pasa a
requerir ese instante para poder construir el DTO completo.

```csharp
public static class Segmentador
{
    public const string Frecuente = "Frecuente";
    public const string Nuevo = "Nuevo";

    public static string Calcular(DateTime fechaAlta, DateTime ahoraUtc) =>
        fechaAlta <= ahoraUtc.AddMonths(-6) ? Frecuente : Nuevo;
}
```

## Riesgos

- **Zona horaria de `FechaAlta` en la base**: la columna no tiene `Kind` explícito
  (EF Core SqlServer la trae como `Unspecified`). BR-01 pide comparar contra "hoy
  (UTC)"; se asume que `FechaAlta` ya se almacena en UTC (como hoy lo hace
  `Semilla.Cargar`, con fechas fijas sin offset). No se normaliza el `Kind` porque
  está fuera del alcance de esta solicitud; se documenta como riesgo conocido, no
  como comportamiento a corregir aquí.
- **Límite de los 6 meses exactos**: se usa `AddMonths(-6)` para el corte, que sigue
  la semántica de calendario de .NET (ajusta meses de distinta longitud). No hay AC
  que pruebe el límite exacto; las pruebas cubren un año (Frecuente) y un mes
  (Nuevo), que es lo que pide el DERCAS.

## Cómo corre esto en el Qubipod (entorno de ejecución)

El endpoint usa EF Core con `UseSqlServer` en producción, y el pod no tiene SQL
Server ni Docker/Testcontainers disponibles. Las pruebas de este cambio no
necesitan SQL Server real:
- Las pruebas de `Segmentador.Calcular` son unitarias puras (sin base de datos).
- Las pruebas del endpoint (`GET /api/clientes/{id}`) usan
  `Microsoft.AspNetCore.Mvc.Testing` (ya referenciado en el proyecto de pruebas)
  con una fábrica de pruebas que reemplaza el `DbContextOptions<ClientesDb>` de
  `UseSqlServer` por `Microsoft.EntityFrameworkCore.InMemory`. Esto no cambia nada
  en `BetaDotnetApi.csproj` (NFR-01 sigue cumplido: el `UseSqlServer` de
  producción no se toca); el paquete `InMemory` se agrega solo al proyecto de
  pruebas (`BetaDotnetApi.Tests.csproj`), que no es el archivo que NFR-01 vigila.
- `TargetFramework` del repo es `net7.0`; el pod trae SDKs 6.0–9.0 además del
  10.0.403 instalado. `dotnet test` debería resolver net7.0 directamente; si el
  pod no tuviera el runtime 7.0 instalado, se corre con
  `dotnet test -p:RollForward=Major` para que use el runtime 10 disponible.
- No se usa `AGENTOS_PG_CONNECTION` ni Postgres: no hace falta una base real para
  probar la regla de segmento ni el contrato HTTP.

## Validación

- `dotnet test` en la raíz del repo (unitarias de `Segmentador` + integración del
  endpoint con InMemory).
- `node .claude/skills/generate-spec/check-contracts.mjs
  openspec/changes/req-2026-005-segmento-cliente-api/contracts/*.contract.json`
  (equivalente local a "check.mjs --role provider de clientes-api").
- `npx openspec validate req-2026-005-segmento-cliente-api --strict`.

Mapa AC → verificación:
- AC-01.01.1, AC-01.01.2: prueba de integración `ClientesEndpointTests` (dos
  clientes sembrados en el momento de la prueba, con `FechaAlta` relativa a
  `DateTime.UtcNow` para no depender de fechas fijas que envejecen).
- AC-01.01.3: la misma clase de prueba, pidiendo un id que no existe en la base
  en memoria del test.
- NFR-01: inspección de `src/BetaDotnetApi/BetaDotnetApi.csproj` — sin
  `PackageReference` nuevas.

## Contratos

### `clientes-api` (nuevo, versión inicial)

- Publica `GET /api/clientes/{id}` con el `ClienteDto` existente
  (`Id`, `NombreCompleto`, `FechaAlta`) más el campo nuevo `Segmento`
  (`"Frecuente"` | `"Nuevo"`), y la respuesta `404` con `ErrorDto`
  (`Codigo`, `Mensaje`) cuando el id no existe.
- Sin consumidores todavía (la solicitud lo declara así); no hay compatibilidad
  que romper.
- **Casing de `clientes-api`: `camelCase`, API nueva (el contrato es la primera
  versión publicada de esta operación), evidencia: `src/BetaDotnetApi/Program.cs`
  no registra `AddHttpJsonOptions` ni ningún `JsonSerializerOptions` propio, así
  que la serialización de las respuestas minimal-API usa el
  `PropertyNamingPolicy` por defecto de ASP.NET Core
  (`Microsoft.AspNetCore.Http.Json.JsonOptions`), que es `camelCase`.** Decisión
  tomada por ausencia de configuración propia, siguiendo la convención del
  framework en el cable.
- `Segmento` se modela como `enum` de dos valores sin `x-extensible` (BR-01 define
  exactamente dos segmentos; si una solicitud futura agrega un tercero, será un
  cambio breaking para los consumidores, lo cual es intencional porque hoy no hay
  ninguno).
