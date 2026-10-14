using LibreriaJoel.Models;

namespace LibreriaJoel.Services;

public static class Validaciones
{
    public static bool ValidarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return false;

        string nombreLimpio = nombre.Trim();
        return nombreLimpio.Length >= 3 && nombre.Length <= 150;
    }

    public static bool validarNombre(string? nombre)
    {
        return ValidarNombre(nombre);
    }

    public static bool ValidarCelular(string? celular)
    {
        if (string.IsNullOrWhiteSpace(celular))
            return true;
        if (celular.Length > 20)
            return false;

        foreach (var caracter in celular)
        {
            if (!char.IsDigit(caracter) && caracter != '+' && caracter != '-' && caracter != ' ')
                return false;
        }

        return true;
    }

    public static string? ValidarUsuario(Usuario usuario, bool requiereContrasena = false, string? contrasena = null)
    {
        if (!ValidarTextoObligatorio(usuario.NombreUsuario, 50))
            return "El nombre de usuario es obligatorio y no puede superar los 50 caracteres.";
        if (!ValidarNombre(usuario.Nombre))
            return "El nombre debe tener entre 3 y 150 caracteres.";
        if (!ValidarLongitudOpcional(usuario.Direccion, 250))
            return "La dirección no puede superar los 250 caracteres.";
        if (!ValidarCelular(usuario.Celular))
            return "El celular solo puede contener números y no superar los 20 caracteres.";
        if (!ValidarLongitudOpcional(usuario.NitCi, 20))
            return "El NIT/CI no puede superar los 20 caracteres.";
        if (usuario.Rol is not "ADMIN" and not "CAJERO")
            return "El rol debe ser ADMIN o CAJERO.";
        if (requiereContrasena && string.IsNullOrWhiteSpace(contrasena))
            return "La contraseña es obligatoria.";
        if (!string.IsNullOrEmpty(contrasena) && contrasena.Length < 12)
            return "La contraseña debe tener al menos 12 caracteres.";
        if (!string.IsNullOrEmpty(contrasena) && contrasena.Length > 128)
            return "La contraseña no puede superar los 128 caracteres.";

        return null;
    }

    public static string? ValidarProducto(Producto producto)
    {
        if (!ValidarTextoObligatorio(producto.Nombre, 150))
            return "El nombre es obligatorio y no puede superar los 150 caracteres.";
        if (!ValidarLongitudOpcional(producto.Lote, 50))
            return "El lote no puede superar los 50 caracteres.";
        if (!ValidarTextoObligatorio(producto.Marca, 80))
            return "La marca es obligatoria y no puede superar los 80 caracteres.";
        if (!ValidarPrecio(producto.CostoEntrada, producto.PrecioVenta))
            return "El precio de venta no puede ser menor al costo de entrada.";
        if (!ValidarStock(producto.Stock))
            return "El stock no puede ser negativo.";
        if (!ValidarTextoObligatorio(producto.UnidadMedida, 20))
            return "La unidad de medida es obligatoria y no puede superar los 20 caracteres.";

        return null;
    }

    public static string? ValidarVenta(Venta venta)
    {
        if (venta.Detalles.Count == 0)
            return "La venta debe incluir al menos un producto.";
        if (!ValidarTextoObligatorio(venta.FormaPago, 20))
            return "La forma de pago es obligatoria y no puede superar los 20 caracteres.";

        return null;
    }

    public static string? ValidarDetalleVenta(DetalleVenta item)
    {
        if (!ValidarIdentificador(item.IdProducto))
            return "Cada producto de la venta debe tener un identificador válido.";
        if (!ValidarCantidad(item.Cantidad))
            return "La cantidad debe ser mayor a cero.";
        if (!ValidarPrecio(0.01m, item.PrecioUnitario))
            return "El precio unitario debe ser mayor a cero.";

        return null;
    }

    public static bool ValidarPrecio(decimal costo, decimal precio)
    {
        return costo > 0 && precio > 0 && precio >= costo;
    }

    public static bool ValidarStock(decimal stock)
    {
        return stock >= 0;
    }

    public static bool ValidarCantidad(decimal cantidad)
    {
        return cantidad > 0;
    }

    public static bool ValidarIdentificador(int id)
    {
        return id > 0;
    }

    private static bool ValidarTextoObligatorio(string? texto, int longitudMaxima)
    {
        return !string.IsNullOrWhiteSpace(texto) && texto.Length <= longitudMaxima;
    }

    private static bool ValidarLongitudOpcional(string? texto, int longitudMaxima)
    {
        return texto is null || texto.Length <= longitudMaxima;
    }
}
