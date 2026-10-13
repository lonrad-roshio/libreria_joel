namespace LibreriaJoel.Services;

public static class Validaciones
{
    public static bool ValidarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return false;

        string nombreLimpio = nombre.Trim();
        return nombreLimpio.Length >= 3;
    }

    public static bool validarNombre(string? nombre)
    {
        return ValidarNombre(nombre);
    }

    public static bool ValidarCelular(string? celular)
    {
        if (string.IsNullOrWhiteSpace(celular))
            return true;

        foreach (var caracter in celular)
        {
            if (!char.IsDigit(caracter) && caracter != '+' && caracter != '-' && caracter != ' ')
                return false;
        }

        return true;
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
}
