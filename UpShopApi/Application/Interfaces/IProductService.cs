using UpShopApi.Domain.Models;

namespace UpShopApi.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductBySkuAsync(string sku);
    }
}
