using BetaDotnetApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ClientesDb>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Principal")));

var app = builder.Build();

// Crea el esquema y los datos de ejemplo al arrancar.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClientesDb>();
    db.Database.EnsureCreated();
    Semilla.Cargar(db);
}

app.MapGet("/api/clientes/{id:int}", async (int id, ClientesDb db) =>
{
    var cliente = await db.Clientes.FindAsync(id);
    return cliente is null
        ? Results.NotFound(new ErrorDto("CLIENTE_NO_ENCONTRADO", $"No existe el cliente {id}."))
        : Results.Ok(ClienteDto.Desde(cliente, DateTime.UtcNow));
});

app.Run();

public partial class Program { }
