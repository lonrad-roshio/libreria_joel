using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Clientes;

public class CreateModel : PageModel
{
    private readonly IClienteService _clienteService;

    public CreateModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [BindProperty]
    public Cliente Cliente { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _clienteService.CrearAsync(Cliente);
            TempData["Mensaje"] = "Cliente registrado correctamente.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
