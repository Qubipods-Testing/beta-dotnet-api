# beta-dotnet-api
Repo sintético (ASP.NET Core, .NET 7) para probar Qubipods en beta (qubitec.ai). Sin datos de clientes.

- `GET /api/clientes/{id}`: un cliente, con su `segmento` (`Frecuente` o `Nuevo`, calculado por el servidor según BR-01) (o 404 con `codigo` y `mensaje`).
- La cadena de conexión de `appsettings.json` apunta a una IP de documentación (203.0.113.10): no existe ninguna base detrás.
- Pruebas: `dotnet test`.
