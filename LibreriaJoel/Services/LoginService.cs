using System.Collections.Concurrent;
using LibreriaJoel.Models;
using LibreriaJoel.Repositories.Interfaces;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibreriaJoel.Services;

public class LoginService : ILoginService
{
    private const int MaximoIntentos = 5;
    private static readonly TimeSpan TiempoBloqueo = TimeSpan.FromMinutes(15);
    private static readonly ConcurrentDictionary<string, IntentosLogin> IntentosPorIp = new();

    private readonly IUsuarioRepository _usuarios;
    private readonly PasswordHasher<Usuario> _hasher = new();
    private readonly ILogger<LoginService> _logger;

    public LoginService(IUsuarioRepository usuarios, ILogger<LoginService> logger)
    {
        _usuarios = usuarios;
        _logger = logger;
    }

    public async Task<Usuario?> ValidarCredencialesAsync(
        string nombreUsuario,
        string contrasena,
        string direccionIp)
    {
        var intentos = IntentosPorIp.GetOrAdd(direccionIp, _ => new IntentosLogin());
        var ahora = DateTime.UtcNow;

        lock (intentos)
        {
            if (intentos.BloqueadoHasta > ahora)
                return null;

            if (intentos.InicioVentana.Add(TiempoBloqueo) < ahora)
            {
                intentos.Cantidad = 0;
                intentos.InicioVentana = ahora;
                intentos.BloqueadoHasta = DateTime.MinValue;
            }
        }

        var usuario = await _usuarios.BuscarPorNombreUsuarioAsync(nombreUsuario.Trim());
        bool credencialesCorrectas = usuario is not null
            && !string.IsNullOrWhiteSpace(usuario.ContrasenaHash)
            && _hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, contrasena)
                != PasswordVerificationResult.Failed;

        lock (intentos)
        {
            if (credencialesCorrectas)
            {
                intentos.Cantidad = 0;
                intentos.BloqueadoHasta = DateTime.MinValue;
                return usuario;
            }

            intentos.Cantidad++;
            if (intentos.Cantidad >= MaximoIntentos)
            {
                intentos.BloqueadoHasta = ahora.Add(TiempoBloqueo);
                _logger.LogWarning("Se bloqueó el acceso temporal desde {DireccionIp}.", direccionIp);
            }
        }

        return null;
    }

    private sealed class IntentosLogin
    {
        public int Cantidad { get; set; }
        public DateTime InicioVentana { get; set; } = DateTime.UtcNow;
        public DateTime BloqueadoHasta { get; set; } = DateTime.MinValue;
    }
}
