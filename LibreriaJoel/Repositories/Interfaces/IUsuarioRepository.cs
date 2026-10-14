using LibreriaJoel.Models;

namespace LibreriaJoel.Repositories.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> BuscarPorNombreUsuarioAsync(string nombreUsuario);
    Task<bool> TieneVentasAsync(int idUsuario);
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? excluirId = null);
}
