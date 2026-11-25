# Diseño: Despedida personalizada en el saludo

## Decisiones (DERCAS aprobado)

### ADR-01 · El endpoint es sin estado y no toca la base de clientes

> Resultado esperado: endpoint `GET /despedida` que responde "Hasta pronto,
> <nombre>", con prueba unitaria.

El DERCAS no pide leer ni validar el nombre contra `ClientesDb`; cualquier texto
recibido en la query string se usa tal cual. Se implementa como una función pura
`Despedidas.Formatear(nombre)` en `src/BetaDotnetApi/Despedida.cs`, invocada desde
un `MapGet("/despedida", ...)` nuevo en `Program.cs`, sin inyectar `ClientesDb`.

```csharp
public static class Despedidas
{
    public static string Formatear(string nombre) => $"Hasta pronto, {nombre}";
}
```

### ADR-02 · `nombre` ausente o vacío es un error de cliente, no un saludo vacío

El DERCAS no detalla el caso borde de un `nombre` ausente. Se decide responder
`400 Bad Request` con `ErrorDto("NOMBRE_REQUERIDO", ...)` en vez de devolver
`Hasta pronto, ` (con el nombre vacío), reutilizando el `ErrorDto` que ya existe
en `src/BetaDotnetApi/Clientes.cs` para mantener un único formato de error en el
servicio. Esto sigue el mismo criterio que ya usa `GET /api/clientes/{id}` para
sus errores (código + mensaje), sin introducir un segundo formato.

## Riesgos

- **Nombre con caracteres especiales en la query string**: no se escapa ni se
  sanea el valor recibido; se refleja tal cual en el cuerpo de texto plano de la
  respuesta. No hay riesgo de inyección porque la respuesta es texto plano
  (`text/plain`), no HTML ni SQL. Si una solicitud futura expone este valor en un
  contexto HTML, ese consumidor deberá escaparlo él mismo.
- **Case del parámetro `nombre`**: ASP.NET Core bindea query strings sin
  distinguir mayúsculas de minúsculas en el nombre del parámetro, así que
  `?Nombre=Ana` y `?nombre=Ana` son equivalentes; no se documenta como
  comportamiento especial porque es el default del framework.

## Cómo corre esto en el Qubipod (entorno de ejecución)

El endpoint no usa `ClientesDb` ni ninguna base de datos: es una función de texto
pura más un `MapGet`. Por eso las pruebas de este cambio no necesitan SQL Server,
Postgres, Docker ni ningún reemplazo de proveedor de base de datos:

- Pruebas unitarias de `Despedidas.Formatear` (sin dependencias).
- Prueba de integración del endpoint con `Microsoft.AspNetCore.Mvc.Testing`
  (`WebApplicationFactory<Program>`, ya referenciado en
  `tests/BetaDotnetApi.Tests.csproj`). Esta prueba arranca el `Program` completo,
  que registra `ClientesDb` con `UseSqlServer`; para que `dotnet test` no intente
  conectar a SQL Server al construir el host, la fábrica de pruebas de este
  cambio reemplaza ese `DbContextOptions<ClientesDb>` por `UseInMemoryDatabase`
  solo en el proyecto de pruebas (no toca `BetaDotnetApi.csproj`). El endpoint
  `/despedida` en sí no usa `ClientesDb`; el reemplazo es solo para poder
  levantar el host de pruebas sin una base real.
- Se agrega el paquete `Microsoft.EntityFrameworkCore.InMemory` (versión 7.0.x)
  solo a `tests/BetaDotnetApi.Tests.csproj`, igual criterio que cualquier cambio
  futuro que necesite levantar el host completo en pruebas.
- `TargetFramework` del repo es `net7.0`; el pod trae SDKs 6.0–9.0 además del
  10.0.403 instalado, así que `dotnet test` resuelve net7.0 directamente sin
  necesitar `RollForward`.
- No se usa `AGENTOS_PG_CONNECTION` ni Postgres: no hace falta una base real para
  probar este endpoint.

## Validación

- `dotnet test` en la raíz del repo (unitarias de `Despedidas` + integración del
  endpoint, además de las pruebas existentes).
- `npx openspec validate req-2026-006-despedida-saludo --strict`.

Mapa AC → verificación:
- AC-01.01.1: prueba de integración `DespedidaEndpointTests` pidiendo
  `GET /despedida?nombre=Ana` y verificando el cuerpo `Hasta pronto, Ana`.
- AC-01.01.2: la misma clase de prueba, pidiendo `GET /despedida` sin `nombre` y
  verificando `400` con `Codigo: "NOMBRE_REQUERIDO"`.
