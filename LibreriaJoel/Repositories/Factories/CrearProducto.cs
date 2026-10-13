using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories.Factories;

public sealed class CrearProducto : RepositoryCreator<Producto>
{
    private readonly DbContext _database;

    public CrearProducto(DbContext database)
    {
        _database = database;
    }

    public override IRepository<Producto> Crear()
    {
        return new ProductoRepository(_database);
    }
}
