using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Clientes;

public class DeleteModel : PageModel
{
    private readonly IClienteService _clienteService;

    public DeleteModel(IClienteService clienteService)
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

    public async Task<IActionResult> OnPostAsync(int id)
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
        return RedirectToPage("Index");
    }
}
