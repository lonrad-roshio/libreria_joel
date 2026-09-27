using LibreriaJoel.Models;
using LibreriaJoel.Repositories;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;

namespace LibreriaJoel.Services;

/// <summary>
/// Reglas de negocio para Cliente. Las Razor Pages solo llaman a este servicio,
/// nunca acceden al repositorio directamente (Principio de Responsabilidad Única
/// y de Inversión de Dependencias).
/// </summary>
public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repositorio;

    public ClienteService(IClienteRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Cliente>> ListarAsync(string? filtro) =>
        string.IsNullOrWhiteSpace(filtro)
            ? await _repositorio.ObtenerTodosAsync()
            : await _repositorio.BuscarPorNombreAsync(filtro.Trim());

    public async Task<Cliente?> ObtenerAsync(int id) => await _repositorio.ObtenerPorIdAsync(id);

    public async Task CrearAsync(Cliente cliente)
    {
        ValidarCliente(cliente);
        await _repositorio.AgregarAsync(cliente);
        await _repositorio.GuardarCambiosAsync();
    }

    public async Task ActualizarAsync(Cliente cliente)
    {
        ValidarCliente(cliente);

        if (!await _repositorio.ExisteAsync(cliente.IdCliente))
            throw new ValidacionException("El cliente que intenta actualizar no existe.");

        _repositorio.Actualizar(cliente);
        await _repositorio.GuardarCambiosAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var cliente = await _repositorio.ObtenerPorIdAsync(id)
            ?? throw new ValidacionException("El cliente que intenta eliminar no existe.");

        if (await _repositorio.TieneVentasAsociadasAsync(id))
            throw new ValidacionException("No se puede eliminar el cliente porque tiene ventas registradas.");

        _repositorio.Eliminar(cliente);
        await _repositorio.GuardarCambiosAsync();
    }

    private static void ValidarCliente(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombre))
            throw new ValidacionException("El nombre del cliente es obligatorio.");

        if (cliente.Nombre.Trim().Length < 3)
            throw new ValidacionException("El nombre del cliente debe tener al menos 3 caracteres.");
    }
}
