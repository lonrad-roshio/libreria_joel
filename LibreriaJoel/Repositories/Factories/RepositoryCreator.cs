using LibreriaJoel.Repositories.Interfaces;

namespace LibreriaJoel.Repositories.Factories;

public abstract class RepositoryCreator<T> where T : class
{
    public abstract IRepository<T> Crear();
}
