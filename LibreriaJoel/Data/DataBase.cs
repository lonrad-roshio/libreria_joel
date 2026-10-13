namespace LibreriaJoel.Data;

public sealed class DataBase
{
    private static readonly DataBase Instance = new DataBase();
    private string? _connectionString;

    private DataBase()
    {
    }

    public static DataBase GetInstancia()
    {
        return Instance;
    }

    public static DataBase getInstancia()
    {
        return GetInstancia();
    }

    public void Configurar(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("La cadena de conexión es obligatoria.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public string ObtenerCadenaConexion()
    {
        return _connectionString
            ?? throw new InvalidOperationException("La conexión a la base de datos no ha sido configurada.");
    }
}
