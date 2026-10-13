using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories.Factories;

public sealed class CrearUsuario : RepositoryCreator<Cliente>
{
    private readonly DbContext _database;
    private readonly IRepository<Venta> _ventas;

    public CrearUsuario(DbContext database, IRepository<Venta> ventas)
    {
        _database = database;
        _ventas = ventas;
    }

    public override IRepository<Cliente> Crear()
    {
        return new ClienteRepository(_database, _ventas);
    }
}
