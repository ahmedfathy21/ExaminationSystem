namespace ExaminationSystem.Common.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task SaveAsync();
}