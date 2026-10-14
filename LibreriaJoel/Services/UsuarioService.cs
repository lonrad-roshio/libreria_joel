using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibreriaJoel.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Usuario>> ListarAsync(string? filtro)
    {
        var usuarios = await _repository.SelectAsync();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.Trim();
            usuarios = usuarios.FindAll(usuario =>
                usuario.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)
                || usuario.NombreUsuario.Contains(texto, StringComparison.OrdinalIgnoreCase)
                || (usuario.NitCi?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        usuarios.Sort((primero, segundo) =>
            string.Compare(primero.Nombre, segundo.Nombre, StringComparison.OrdinalIgnoreCase));
        return usuarios;
    }

    public Task<Usuario?> ObtenerAsync(int id)
    {
        return _repository.FindAsync(id);
    }

    public async Task CrearAsync(Usuario usuario, string contrasena)
    {
        Validar(usuario, requiereContrasena: true, contrasena);
        await ValidarNombreUsuarioUnicoAsync(usuario.NombreUsuario);
        usuario.NombreUsuario = usuario.NombreUsuario.Trim();
        usuario.Nombre = usuario.Nombre.Trim();
        usuario.ContrasenaHash = _passwordHasher.HashPassword(usuario, contrasena);
        await _repository.InsertAsync(usuario);
    }

    public async Task ActualizarAsync(Usuario usuario, string? contrasena)
    {
        var existente = await _repository.FindAsync(usuario.IdUsuario)
            ?? throw new ValidacionException("El usuario no existe.");

        Validar(usuario, requiereContrasena: false, contrasena);
        await ValidarNombreUsuarioUnicoAsync(usuario.NombreUsuario, usuario.IdUsuario);
        existente.NombreUsuario = usuario.NombreUsuario.Trim();
        existente.Nombre = usuario.Nombre.Trim();
        existente.Direccion = usuario.Direccion?.Trim();
        existente.Celular = usuario.Celular?.Trim();
        existente.NitCi = usuario.NitCi?.Trim();
        existente.Rol = usuario.Rol;
        if (!string.IsNullOrWhiteSpace(contrasena))
            existente.ContrasenaHash = _passwordHasher.HashPassword(existente, contrasena);

        await _repository.UpdateAsync(existente);
    }

    public async Task EliminarAsync(int id)
    {
        var usuario = await _repository.FindAsync(id)
            ?? throw new ValidacionException("El usuario no existe.");
        if (await _repository.TieneVentasAsync(id))
            throw new ValidacionException("No se puede eliminar un usuario que tiene ventas registradas.");

        await _repository.DeleteAsync(usuario);
    }

    private static void Validar(Usuario usuario, bool requiereContrasena, string? contrasena)
    {
        var error = Validaciones.ValidarUsuario(usuario, requiereContrasena, contrasena);
        if (error is not null)
            throw new ValidacionException(error);
    }

    private async Task ValidarNombreUsuarioUnicoAsync(string nombreUsuario, int? excluirId = null)
    {
        if (await _repository.ExisteNombreUsuarioAsync(nombreUsuario.Trim(), excluirId))
            throw new ValidacionException("Ese nombre de usuario ya está registrado.");
    }
}
