using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Productos;

public class CreateModel : PageModel
{
    private readonly IProductoService _productoService;

    public CreateModel(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [BindProperty]
    public Producto Producto { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _productoService.CrearAsync(Producto);
            TempData["Mensaje"] = "Producto registrado correctamente.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
