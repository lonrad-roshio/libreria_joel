using LibreriaJoel.Data;
using LibreriaJoel.Repositories;
using LibreriaJoel.Services;
using LibreriaJoel.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Servicios de infraestructura ----------
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("LibreriaJoelConnection");
builder.Services.AddDbContext<LibreriaJoelContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ---------- Inyección de dependencias (Principio D de SOLID: Dependency Inversion) ----------
// Las Razor Pages dependen de abstracciones (interfaces), nunca de implementaciones concretas.
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
