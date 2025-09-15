using Newtonsoft.Json;
using System.Net.Http.Json;
using UpShopApi.Domain.Models;

namespace UpShopTests.Infrastructure
{
    public class UpShopApiClient
    {
        private readonly HttpClient _client;
        public UpShopApiClient(HttpClient client) { _client = client; }

        // Calls API endpoint to get all products
        public async Task<(IEnumerable<Product>? products, ApiError? error)> GetAllProductsAsync()
        {
            var response = await _client.GetAsync("/api/products");
            return await HandleResponseAsync<IEnumerable<Product>>(response);
        }

        // Calls API endpoint to get a single product by SKU
        public async Task<(Product? product, ApiError? error)> GetBySkuAsync(string sku)
        {
            var response = await _client.GetAsync($"/api/products/{sku}");
            return await HandleResponseAsync<Product>(response);
        }

        private async Task<(T? data, ApiError? error)> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<T>(json);
                return (data, null);
            }

            var apiError = JsonConvert.DeserializeObject<ApiError>(json)
                           ?? new ApiError
                           {
                               StatusCode = (int)response.StatusCode,
                               ErrorCode = "UNKNOWN",
                               Message = "An unknown error occurred."
                           };
            return (default, apiError);
        }
    }
}
