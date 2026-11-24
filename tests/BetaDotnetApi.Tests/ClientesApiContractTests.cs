using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BetaDotnetApi;
using Xunit;

namespace BetaDotnetApi.Tests;

/// <summary>
/// Una prueba por ejemplo del contrato `clientes-api` contra el pipeline HTTP real; deja lo
/// observado en .agentos/contracts/observed/clientes-api/&lt;ejemplo&gt;.json para `check.mjs`.
/// </summary>
public class ClientesApiContractTests
{
    private const string Slug = "clientes-api";

    [Theory]
    [InlineData("cliente-frecuente", 101, "Ana Pérez", "2025-10-01T00:00:00Z")]
    [InlineData("cliente-nuevo", 102, "Luis Gómez", "2026-10-20T00:00:00Z")]
    public async Task Ejemplo_de_cliente_encontrado(string ejemplo, int id, string nombreCompleto, string fechaAltaIso)
    {
        using var factory = new ClientesApiFactory();
        var fechaAlta = DateTime.Parse(fechaAltaIso, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        factory.Seed(new Cliente { Id = id, NombreCompleto = nombreCompleto, FechaAlta = fechaAlta });
        var client = factory.CreateClient();

        var respuesta = await client.GetAsync($"/api/clientes/{id}");
        await GuardarObservado(ejemplo, respuesta);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
    }

    [Fact]
    public async Task Ejemplo_de_cliente_inexistente()
    {
        using var factory = new ClientesApiFactory();
        factory.Seed();
        var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/api/clientes/999");
        await GuardarObservado("cliente-inexistente", respuesta);

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
    }

    private static async Task GuardarObservado(string ejemplo, HttpResponseMessage respuesta)
    {
        var cuerpoCrudo = await respuesta.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<JsonElement>(cuerpoCrudo);
        var observado = new { status = (int)respuesta.StatusCode, body };

        var dir = Path.Combine(EncontrarRaizDelRepo(), ".agentos", "contracts", "observed", Slug);
        Directory.CreateDirectory(dir);
        await File.WriteAllTextAsync(Path.Combine(dir, $"{ejemplo}.json"), JsonSerializer.Serialize(observado));
    }

    private static string EncontrarRaizDelRepo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, ".ai-stack")))
            dir = dir.Parent;
        return dir?.FullName
            ?? throw new InvalidOperationException("No se encontró la raíz del repo (carpeta .ai-stack) desde " + AppContext.BaseDirectory);
    }
}
