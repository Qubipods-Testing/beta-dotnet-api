using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BetaDotnetApi;
using Xunit;

namespace BetaDotnetApi.Tests;

public class ClientesEndpointTests
{
    private static readonly JsonSerializerOptions JsonOpciones = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task AC_01_01_1_cliente_con_alta_de_hace_un_anio_devuelve_segmento_frecuente()
    {
        using var factory = new ClientesApiFactory();
        var ahora = DateTime.UtcNow;
        factory.Seed(new Cliente { Id = 1, NombreCompleto = "Ana Pérez", FechaAlta = ahora.AddYears(-1) });
        var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/api/clientes/1");
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<ClienteDto>(JsonOpciones);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Equal(Segmentador.Frecuente, cuerpo!.Segmento);
    }

    [Fact]
    public async Task AC_01_01_2_cliente_con_alta_de_hace_un_mes_devuelve_segmento_nuevo()
    {
        using var factory = new ClientesApiFactory();
        var ahora = DateTime.UtcNow;
        factory.Seed(new Cliente { Id = 2, NombreCompleto = "Luis Gómez", FechaAlta = ahora.AddMonths(-1) });
        var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/api/clientes/2");
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<ClienteDto>(JsonOpciones);

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Equal(Segmentador.Nuevo, cuerpo!.Segmento);
    }

    [Fact]
    public async Task AC_01_01_3_id_inexistente_devuelve_404_con_codigo_cliente_no_encontrado()
    {
        using var factory = new ClientesApiFactory();
        factory.Seed();
        var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/api/clientes/999");
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<ErrorDto>(JsonOpciones);

        Assert.Equal(HttpStatusCode.NotFound, respuesta.StatusCode);
        Assert.Equal("CLIENTE_NO_ENCONTRADO", cuerpo!.Codigo);
    }
}
