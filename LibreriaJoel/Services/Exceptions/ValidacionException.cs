namespace LibreriaJoel.Services.Exceptions;

/// <summary>
/// Excepción de dominio para errores de validación de reglas de negocio.
/// Permite separar errores esperados (mostrar mensaje al usuario) de errores técnicos.
/// </summary>
public class ValidacionException : Exception
{
    public ValidacionException(string mensaje) : base(mensaje) { }
}
