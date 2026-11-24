using System;
using System.Linq;
using BetaDotnetApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BetaDotnetApi.Tests;

/// <summary>
/// Reemplaza el `UseSqlServer` de producción por una base en memoria ANTES de que el host
/// arranque, para que las pruebas nunca toquen la cadena de conexión real de appsettings.json.
/// </summary>
public class ClientesApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ClientesDb>));
            if (descriptor is not null) services.Remove(descriptor);
            services.AddDbContext<ClientesDb>(o => o.UseInMemoryDatabase(_dbName));
        });
    }

    public void Seed(params Cliente[] clientes)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ClientesDb>();
        db.Clientes.RemoveRange(db.Clientes);
        db.Clientes.AddRange(clientes);
        db.SaveChanges();
    }
}
