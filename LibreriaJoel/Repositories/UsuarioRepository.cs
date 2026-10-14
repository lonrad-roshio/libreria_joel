using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibreriaJoel.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    private readonly DbContext _database;

    public UsuarioRepository(DbContext database) : base(database)
    {
        _database = database;
    }

    public Task<Usuario?> BuscarPorNombreUsuarioAsync(string nombreUsuario)
    {
        return _database.Set<Usuario>()
            .SingleOrDefaultAsync(usuario => usuario.NombreUsuario == nombreUsuario);
    }

    public Task<bool> TieneVentasAsync(int idUsuario)
    {
        return _database.Set<Venta>().AnyAsync(venta => venta.IdUsuario == idUsuario);
    }

    public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? excluirId = null)
    {
        return _database.Set<Usuario>().AnyAsync(usuario =>
            usuario.NombreUsuario == nombreUsuario
            && (!excluirId.HasValue || usuario.IdUsuario != excluirId.Value));
    }
}
