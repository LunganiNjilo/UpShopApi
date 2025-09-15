using System.Net;
using UpShopApi.Application.Interfaces;
using UpShopApi.Domain.Models;
using UpShopApi.Exceptions;

namespace UpShopApi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = await _repository.GetAllAsync();

            if (!products.Any())
                throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, "No products found.");

            return products;
        }

        public async Task<Product?> GetProductBySkuAsync(string sku)
        {
            ValidateSku(sku);

            var product = (await _repository.GetAllAsync())
                          .FirstOrDefault(p => string.Equals(p.Sku, sku, StringComparison.OrdinalIgnoreCase));

            return product ?? throw new ApiException((int)HttpStatusCode.NotFound, ErrorType.NotFound, $"Product with SKU '{sku}' not found.");
        }

        private static void ValidateSku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, "SKU cannot be empty.");
        }
    }
}
