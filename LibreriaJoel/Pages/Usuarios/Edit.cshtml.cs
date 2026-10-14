using LibreriaJoel.Models;
using LibreriaJoel.Services.Exceptions;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Usuarios;

[Authorize(Roles = "ADMIN")]
public class EditModel : PageModel
{
    private readonly IUsuarioService _usuarioService;

    public EditModel(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [BindProperty]
    public Usuario Usuario { get; set; } = new();

    [BindProperty]
    public string? Contrasena { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var usuario = await _usuarioService.ObtenerAsync(id);
        if (usuario is null) return RedirectToPage("Index");

        Usuario = usuario;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _usuarioService.ActualizarAsync(Usuario, Contrasena);
            TempData["Mensaje"] = "Usuario actualizado correctamente.";
            return RedirectToPage("Index");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
