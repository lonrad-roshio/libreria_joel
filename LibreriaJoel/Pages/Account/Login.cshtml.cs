using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibreriaJoel.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly ILoginService _loginService;

    public LoginModel(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [BindProperty]
    public DatosLogin Datos { get; set; } = new DatosLogin();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
        Response.Headers.CacheControl = "no-store";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Response.Headers.CacheControl = "no-store";

        if (!ModelState.IsValid)
            return Page();

        string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
        var usuario = await _loginService.ValidarCredencialesAsync(
            Datos.NombreUsuario,
            Datos.Contrasena,
            ip);

        if (usuario is null)
        {
            Datos.Contrasena = string.Empty;
            ModelState.AddModelError(string.Empty, "No se pudo iniciar sesión. Revisa tus datos e inténtalo de nuevo.");
            return Page();
        }

        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()));
        claims.Add(new Claim(ClaimTypes.Name, usuario.NombreUsuario));
        claims.Add(new Claim(ClaimTypes.GivenName, usuario.Nombre));
        claims.Add(new Claim(ClaimTypes.Role, usuario.Rol));
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identidad);
        var propiedades = new AuthenticationProperties();
        propiedades.IsPersistent = false;
        propiedades.AllowRefresh = false;

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            propiedades);

        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Index");
    }

    public class DatosLogin
    {
        [Required(ErrorMessage = "Escribe tu usuario.")]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe tu contraseña.")]
        [StringLength(128)]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;
    }
}
