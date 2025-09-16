using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UpShopApi.Domain.Models;

namespace UpShopApi.Benchmarks
{
    [TestFixture]
    public class ProductServiceBenchmarkTests : IDisposable
    {
        private HttpClient _client = null!;
        // Production-like thresholds
        private const int GetAllProductsThresholdMs = 150;  // ≤150 ms
        private const int GetProductBySkuThresholdMs = 50;  // ≤50 ms
        private const int ConcurrentRequestThresholdMs = 200; // ≤200 ms under light concurrency

        [SetUp]
        public void Setup()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000") // Adjust if needed
            };
        }

        [Test]
        public async Task Benchmark_GetAllProducts()
        {
            var stopwatch = Stopwatch.StartNew();

            var products = await _client.GetFromJsonAsync<List<Product>>("/api/products");

            stopwatch.Stop();
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThanOrEqualTo(GetAllProductsThresholdMs),
                $"GetAllProducts exceeded threshold of {GetAllProductsThresholdMs} ms");
        }

        [TestCase("SKU123")]
        [TestCase("SKU456")]
        public async Task Benchmark_GetProductBySku(string sku)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await _client.GetAsync($"/api/products/{sku}");
            stopwatch.Stop();

            var body = await response.Content.ReadAsStringAsync();

            // Try to deserialize into ApiError (expected for invalid SKU)
            var error = JsonSerializer.Deserialize<ApiError>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.That(response.IsSuccessStatusCode, Is.False, "Expected an error but got success.");
            Assert.That(error, Is.Not.Null, "Error response did not bind correctly.");
            Assert.That(error.ErrorCode, Is.Not.Null.And.Not.Empty, "Error code was missing.");
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(GetProductBySkuThresholdMs),
                $"GetProductBySku({sku}) exceeded threshold of {GetProductBySkuThresholdMs} ms");
        }


        [Test]
        public async Task Benchmark_ConcurrentRequests_GetProductBySku()
        {
            var skus = new[] { "SKU123", "SKU456", "SKU789" };

            // Run all requests in parallel
            var tasks = skus.Select(async sku =>
            {
                var stopwatch = Stopwatch.StartNew();

                var response = await _client.GetAsync($"/api/products/{sku}");
                stopwatch.Stop();

                object? resultModel;
                if (response.IsSuccessStatusCode)
                {
                    resultModel = await response.Content.ReadFromJsonAsync<Product>();
                }
                else
                {
                    resultModel = await response.Content.ReadFromJsonAsync<ApiError>();
                }

                return new
                {
                    Sku = sku,
                    Response = response,
                    Result = resultModel,
                    ElapsedMs = stopwatch.ElapsedMilliseconds
                };
            });

            var results = await Task.WhenAll(tasks);

            // Assertions: correctness first
            foreach (var result in results)
            {
                if (result.Response.IsSuccessStatusCode)
                {
                    Assert.That(result.Result, Is.InstanceOf<Product>(), $"Expected Product for {result.Sku}");
                }
                else
                {
                    Assert.That(result.Result, Is.InstanceOf<ApiError>(), $"Expected ApiError for {result.Sku}");
                }
            }

            // Assertions: performance second
            foreach (var result in results)
            {
                Console.WriteLine($"Concurrent GetProductBySku({result.Sku}) executed in {result.ElapsedMs} ms");

                Assert.That(result.ElapsedMs,
                    Is.LessThanOrEqualTo(ConcurrentRequestThresholdMs),
                    $"Concurrent request for {result.Sku} exceeded threshold of {ConcurrentRequestThresholdMs} ms");
            }
        }


        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
