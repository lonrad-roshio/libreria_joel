using LibreriaJoel.Models;

namespace LibreriaJoel.Services.Interfaces;

public interface ILoginService
{
    Task<Usuario?> ValidarCredencialesAsync(string nombreUsuario, string contrasena, string direccionIp);
}
