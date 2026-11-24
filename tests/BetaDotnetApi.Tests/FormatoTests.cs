using BetaDotnetApi;
using Xunit;

namespace BetaDotnetApi.Tests;

public class FormatoTests
{
    [Theory]
    [InlineData("Ana Pérez", "AP")]
    [InlineData("luis  gómez", "LG")]
    public void Iniciales_toma_la_primera_letra_de_cada_palabra(string nombre, string esperado) =>
        Assert.Equal(esperado, Formato.Iniciales(nombre));
}
