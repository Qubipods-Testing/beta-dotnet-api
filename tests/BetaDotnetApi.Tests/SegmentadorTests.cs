using System;
using BetaDotnetApi;
using Xunit;

namespace BetaDotnetApi.Tests;

public class SegmentadorTests
{
    [Fact]
    public void Cliente_con_alta_de_hace_un_anio_es_frecuente()
    {
        var ahoraUtc = new DateTime(2026, 11, 24, 0, 0, 0, DateTimeKind.Utc);
        var fechaAlta = ahoraUtc.AddYears(-1);

        Assert.Equal(Segmentador.Frecuente, Segmentador.Calcular(fechaAlta, ahoraUtc));
    }

    [Fact]
    public void Cliente_con_alta_de_hace_un_mes_es_nuevo()
    {
        var ahoraUtc = new DateTime(2026, 11, 24, 0, 0, 0, DateTimeKind.Utc);
        var fechaAlta = ahoraUtc.AddMonths(-1);

        Assert.Equal(Segmentador.Nuevo, Segmentador.Calcular(fechaAlta, ahoraUtc));
    }
}
