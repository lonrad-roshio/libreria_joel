using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Clientes;

public class EditModel : PageModel
{
    private readonly IClienteService _clienteService;

    public EditModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [BindProperty]
    public Cliente Cliente { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var cliente = await _clienteService.ObtenerAsync(id);
        if (cliente is null) return RedirectToPage("Index");

        Cliente = cliente;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _clienteService.ActualizarAsync(Cliente);
            TempData["Mensaje"] = "Cliente actualizado correctamente.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
