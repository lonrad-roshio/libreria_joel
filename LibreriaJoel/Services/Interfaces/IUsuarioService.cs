using LibreriaJoel.Models;

namespace LibreriaJoel.Services.Interfaces;

public interface IUsuarioService
{
    Task<List<Usuario>> ListarAsync(string? filtro);
    Task<Usuario?> ObtenerAsync(int id);
    Task CrearAsync(Usuario usuario, string contrasena);
    Task ActualizarAsync(Usuario usuario, string? contrasena);
    Task EliminarAsync(int id);
}
