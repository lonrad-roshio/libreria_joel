using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories.Factories;

public sealed class CrearUsuario : RepositoryCreator<Usuario>
{
    private readonly DbContext _database;

    public CrearUsuario(DbContext database)
    {
        _database = database;
    }

    public override IRepository<Usuario> Crear()
    {
        return new UsuarioRepository(_database);
    }
}
