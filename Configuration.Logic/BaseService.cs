namespace Configuration.Logic;

public class BaseService<TEntity> : IBaseService<TEntity>
{
    /// <summary>
    /// Интерфейс базового репозитория.
    /// </summary>
    IRepositoryBase<TEntity> Repository { get; }
}