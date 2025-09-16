using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using UpShopApi.Domain.Models;

namespace UpShopApi.FunctionalTests
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private HttpClient _client = null!;

        [SetUp]
        public void Setup()
        {
            // Point this at the running API (local or Docker)
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnProducts()
        {
            var response = await _client.GetAsync("/api/products");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var products = await response.Content.ReadFromJsonAsync<List<Product>>();

            Assert.That(products, Is.Not.Null);
            Assert.That(products, Is.Not.Empty);
        }

        [TestCase("SKU-0014")]
        public async Task GetProductBySku_ValidSku_ShouldReturnProduct(string sku)
        {
            var response = await _client.GetAsync($"/api/products/{sku}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var product = await response.Content.ReadFromJsonAsync<Product>();
            Assert.That(product, Is.Not.Null);
            Assert.That(product!.Sku, Is.EqualTo(sku));
        }

        [TestCase("INVALID-SKU")]
        public async Task GetProductBySku_InvalidSku_ShouldReturnApiError(string sku)
        {
            var response = await _client.GetAsync($"/api/products/{sku}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.ErrorCode, Is.EqualTo("AV404"));
        }
    }
}
