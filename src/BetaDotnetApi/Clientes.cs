using Microsoft.EntityFrameworkCore;

namespace BetaDotnetApi;

public class Cliente
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = "";
    public DateTime FechaAlta { get; set; }
}

public class ClientesDb : DbContext
{
    public ClientesDb(DbContextOptions<ClientesDb> options) : base(options) { }
    public DbSet<Cliente> Clientes => Set<Cliente>();
}

/// <summary>Respuesta de GET /api/clientes/{id}. System.Text.Json la serializa con los nombres por defecto de ASP.NET Core.</summary>
public record ClienteDto(int Id, string NombreCompleto, DateTime FechaAlta)
{
    public static ClienteDto Desde(Cliente c) => new(c.Id, c.NombreCompleto, c.FechaAlta);
}

public record ErrorDto(string Codigo, string Mensaje);

public static class Semilla
{
    public static void Cargar(ClientesDb db)
    {
        if (db.Clientes.Any()) return;
        db.Clientes.AddRange(
            new Cliente { NombreCompleto = "Ana Pérez", FechaAlta = new DateTime(2025, 3, 1) },
            new Cliente { NombreCompleto = "Luis Gómez", FechaAlta = new DateTime(2025, 7, 15) });
        db.SaveChanges();
    }
}

public static class Formato
{
    /// <summary>Iniciales de un nombre: «Ana Pérez» → «AP».</summary>
    public static string Iniciales(string nombre) =>
        string.Concat(nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(p => char.ToUpperInvariant(p[0])));
}
