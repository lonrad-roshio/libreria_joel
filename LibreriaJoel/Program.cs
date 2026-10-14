using LibreriaJoel.Data;
using LibreriaJoel.Models;
using LibreriaJoel.Repositories;
using LibreriaJoel.Repositories.Interfaces;
using LibreriaJoel.Services;
using LibreriaJoel.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LibreriaJoelConnection")
    ?? throw new InvalidOperationException("No se encontró ConnectionStrings:LibreriaJoelConnection.");
var dataBase = DataBase.GetInstancia();
dataBase.Configurar(connectionString);

builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });
builder.Services.AddDbContext<LibreriaJoelContext>(ConfigurarBaseDeDatos);
builder.Services.AddScoped<DbContext>(ObtenerContexto);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(ConfigurarCookie);
builder.Services.AddAuthorization(ConfigurarAutorizacion);
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IVentaService, VentaService>();

var app = builder.Build();
await InicializarUsuarioAdministradorAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();

void ConfigurarBaseDeDatos(DbContextOptionsBuilder options)
{
    options.UseMySql(
        dataBase.ObtenerCadenaConexion(),
        new MySqlServerVersion(new Version(8, 0, 0)));
}

void ConfigurarCookie(CookieAuthenticationOptions options)
{
    options.Cookie.Name = "LibreriaJoel.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;
}

void ConfigurarAutorizacion(AuthorizationOptions options)
{
    var politica = new AuthorizationPolicyBuilder();
    politica.RequireAuthenticatedUser();
    options.FallbackPolicy = politica.Build();
}

DbContext ObtenerContexto(IServiceProvider servicios)
{
    return servicios.GetRequiredService<LibreriaJoelContext>();
}

async Task InicializarUsuarioAdministradorAsync(WebApplication aplicacion)
{
    await using var scope = aplicacion.Services.CreateAsyncScope();
    var contexto = scope.ServiceProvider.GetRequiredService<LibreriaJoelContext>();
    if (await contexto.Usuarios.AnyAsync(usuario =>
        usuario.Rol == "ADMIN" && usuario.ContrasenaHash != string.Empty))
        return;

    var nombreUsuario = aplicacion.Configuration["Login:Username"]?.Trim();
    var contrasena = aplicacion.Configuration["Login:Password"];
    if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasena))
    {
        throw new InvalidOperationException(
            "No existen usuarios. Configura Login__Username y Login__Password para crear el primer administrador.");
    }

    var nombre = aplicacion.Configuration["Login:DisplayName"]?.Trim();
    var administrador = new Usuario
    {
        NombreUsuario = nombreUsuario,
        Nombre = string.IsNullOrWhiteSpace(nombre) ? nombreUsuario : nombre,
        Rol = "ADMIN"
    };
    var error = Validaciones.ValidarUsuario(administrador, requiereContrasena: true, contrasena);
    if (error is not null)
        throw new InvalidOperationException($"No se pudo crear el primer administrador: {error}");
    if (await contexto.Usuarios.AnyAsync(usuario => usuario.NombreUsuario == nombreUsuario))
        throw new InvalidOperationException(
            $"El nombre de usuario '{nombreUsuario}' ya existe pero no tiene una cuenta administradora con contraseña.");
    administrador.ContrasenaHash = new PasswordHasher<Usuario>()
        .HashPassword(administrador, contrasena);
    contexto.Usuarios.Add(administrador);
    await contexto.SaveChangesAsync();
}