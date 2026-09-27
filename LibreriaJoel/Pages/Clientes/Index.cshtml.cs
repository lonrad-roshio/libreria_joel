using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Clientes;

public class IndexModel : PageModel
{
    private readonly IClienteService _clienteService;

    public IndexModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    public List<Cliente> Clientes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Filtro { get; set; }

    public async Task OnGetAsync()
    {
        Clientes = await _clienteService.ListarAsync(Filtro);
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        try
        {
            await _clienteService.EliminarAsync(id);
            TempData["Mensaje"] = "Cliente eliminado correctamente.";
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}
