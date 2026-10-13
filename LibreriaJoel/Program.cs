using LibreriaJoel.Data;
using LibreriaJoel.Repositories;
using LibreriaJoel.Repositories.Interfaces;
using LibreriaJoel.Services;
using LibreriaJoel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LibreriaJoelConnection")
    ?? throw new InvalidOperationException("No se encontró ConnectionStrings:LibreriaJoelConnection.");
var dataBase = DataBase.GetInstancia();
dataBase.Configurar(connectionString);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<LibreriaJoelContext>(ConfigurarBaseDeDatos);
builder.Services.AddScoped<DbContext>(ObtenerContexto);

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IVentaService, VentaService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();

void ConfigurarBaseDeDatos(DbContextOptionsBuilder options)
{
    options.UseMySql(
        dataBase.ObtenerCadenaConexion(),
        new MySqlServerVersion(new Version(8, 0, 0)));
}

DbContext ObtenerContexto(IServiceProvider servicios)
{
    return servicios.GetRequiredService<LibreriaJoelContext>();
}