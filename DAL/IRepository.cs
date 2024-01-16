namespace GarageProject.DAL;

public interface IRepository<T>
{
    Task<T> GetById(long id);
    Task<T> GetByEmail(string email); //implementation is optional based on entity type
    Task<IEnumerable<T>> GetAll();

    Task<IEnumerable<T>> GetList(List<long> ids);
    Task<bool> Add(T entity);
    Task<bool> Update(long id, T entity);
    Task<bool> Delete(long id);
}