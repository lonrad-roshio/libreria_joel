using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;

namespace LibreriaJoel.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Cliente>> ListarAsync(string? filtro)
    {
        var clientes = await _repository.SelectAsync();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            var texto = filtro.Trim();
            var encontrados = new List<Cliente>();
            foreach (var cliente in clientes)
            {
                bool coincideNombre = cliente.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase);
                bool coincideNit = false;
                if (cliente.NitCi != null)
                    coincideNit = cliente.NitCi.Contains(texto, StringComparison.OrdinalIgnoreCase);

                if (coincideNombre || coincideNit)
                    encontrados.Add(cliente);
            }

            clientes = encontrados;
        }

        clientes.Sort(CompararPorNombre);
        return clientes;
    }

    public Task<Cliente?> ObtenerAsync(int id)
    {
        return _repository.FindAsync(id);
    }

    public async Task CrearAsync(Cliente cliente)
    {
        Validar(cliente);
        await _repository.InsertAsync(cliente);
    }

    public async Task ActualizarAsync(Cliente cliente)
    {
        var existente = await _repository.FindAsync(cliente.IdCliente)
            ?? throw new ValidacionException("El cliente no existe.");

        Validar(cliente);
        existente.Nombre = cliente.Nombre.Trim();
        existente.Direccion = cliente.Direccion?.Trim();
        existente.Celular = cliente.Celular?.Trim();
        existente.NitCi = cliente.NitCi?.Trim();
        await _repository.UpdateAsync(existente);
    }

    public async Task EliminarAsync(int id)
    {
        var cliente = await _repository.FindAsync(id)
            ?? throw new ValidacionException("El cliente no existe.");

        if (await _repository.TieneVentasAsync(id))
            throw new ValidacionException("No se puede eliminar un cliente que tiene ventas registradas.");

        await _repository.DeleteAsync(cliente);
    }

    private static void Validar(Cliente cliente)
    {
        if (!Validaciones.ValidarNombre(cliente.Nombre))
            throw new ValidacionException("El nombre debe tener al menos 3 caracteres.");
        if (!Validaciones.ValidarCelular(cliente.Celular))
            throw new ValidacionException("El celular solo puede contener números.");
    }

    private static int CompararPorNombre(Cliente primero, Cliente segundo)
    {
        return string.Compare(primero.Nombre, segundo.Nombre, StringComparison.OrdinalIgnoreCase);
    }
}
