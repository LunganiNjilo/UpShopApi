using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using UpShopApi.Domain.Models;
using UpShopTests.Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UpShopTests.Tests
{
    internal class ProductsControllerTests : TestBase
    {
        private UpShopApiClient _upShopApiClient;

        [SetUp]
        public void Setup()
        {
            _upShopApiClient = CreateApiClient<UpShopApiClient>();
            ProductRepositoryMock.ClearReceivedCalls();
        }

        [Test]
        public async Task GetProducts_ReturnsMockedProducts()
        {
            // Arrange
            var expected = new[]
                 {
                    new Product { Id = "1", Sku = "SKU-001", Name = "A", Price = 10, AvailableQuantity = 5 },
                    new Product { Id = "2", Sku = "SKU-002", Name = "B", Price = 20, AvailableQuantity = 3 }
           };


            ProductRepositoryMock.GetAllAsync()
                .Returns(new[]
                {
                    new Product { Id = "1", Sku = "SKU-001", Name = "A", Price = 10, AvailableQuantity = 5 },
                    new Product { Id = "2", Sku = "SKU-002", Name = "B", Price = 20, AvailableQuantity = 3 }
                });

            // Act
            var response = await _upShopApiClient.GetAllProductsAsync();

            // Assert
            response.products.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetProducts_GetAllProductsAndUnexpectedErrorOccurs_ReturnStatus500()
        {
            // Arrange
            var sku = "SKU-002";
            var expected = new ApiError
            {
                StatusCode = 500,
                ErrorCode = "AV500",
                Message = "An unexpected error occurred."
            };

            ProductRepositoryMock.GetAllAsync()
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var response = await _upShopApiClient.GetAllProductsAsync();

            // Assert
            response.error.Should().BeEquivalentTo(expected);
        }


        [Test]
        public async Task GetProductsBySku_ReturnsMockedProductsResults()
        {
            // Arrange
            var expected = new Product { Id = "2", Sku = "SKU-002", Name = "B", Price = 20, AvailableQuantity = 3 };
            var sku = "SKU-002";

            ProductRepositoryMock.GetAllAsync()
                .Returns(new[]
                {
                    new Product { Id = "1", Sku = "SKU-001", Name = "A", Price = 10, AvailableQuantity = 5 },
                    new Product { Id = "2", Sku = "SKU-002", Name = "B", Price = 20, AvailableQuantity = 3 }
                });

            // Act
            var response = await _upShopApiClient.GetBySkuAsync(sku);

            // Assert
            response.product.Should().BeEquivalentTo(expected);
        }


        [Test]
        public async Task GetProductsBySku_GivenValidSkuAndUnexpectedErrorOccurs_ReturnStatus500()
        {
            // Arrange
            var sku = "SKU-002";
            var expected = new ApiError
            {
                StatusCode = 500,
                ErrorCode = "AV500",
                Message = "An unexpected error occurred."
            };

            ProductRepositoryMock.GetAllAsync()
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var response = await _upShopApiClient.GetBySkuAsync(sku);

            // Assert
            response.error.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetProductsBySku_GivenInvalidSku_ReturnStatus404()
        {
            // Arrange
            var sku = "SKU-0000";
            var expected = new ApiError
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                ErrorCode = "AV404",
                Message = $"Product with SKU '{sku}' not found."
            };

            ProductRepositoryMock.GetAllAsync()
                .Returns(new[]
                {
                    new Product { Id = "1", Sku = "SKU-001", Name = "A", Price = 10, AvailableQuantity = 5 },
                    new Product { Id = "2", Sku = "SKU-002", Name = "B", Price = 20, AvailableQuantity = 3 }
                });

            // Act
            var response = await _upShopApiClient.GetBySkuAsync(sku);

            // Assert
            response.error.Should().BeEquivalentTo(expected);
        }

    }
}
