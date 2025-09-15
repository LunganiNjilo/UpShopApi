using UpShopApi.Domain;

namespace UpShopApi.Application.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
    }
}
