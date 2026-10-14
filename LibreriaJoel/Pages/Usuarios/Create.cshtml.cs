using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Usuarios;

[Authorize(Roles = "ADMIN")]
public class CreateModel : PageModel
{
    private readonly IUsuarioService _usuarioService;

    public CreateModel(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [BindProperty]
    public Usuario Usuario { get; set; } = new();

    [BindProperty]
    public string Contrasena { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _usuarioService.CrearAsync(Usuario, Contrasena);
            TempData["Mensaje"] = "Usuario registrado correctamente.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
