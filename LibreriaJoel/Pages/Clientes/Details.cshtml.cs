using LibreriaJoel.Models;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Clientes;

public class DetailsModel : PageModel
{
    private readonly IClienteService _clienteService;

    public DetailsModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    public Cliente? Cliente { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Cliente = await _clienteService.ObtenerAsync(id);
        if (Cliente is null) return RedirectToPage("Index");
        return Page();
    }
}
