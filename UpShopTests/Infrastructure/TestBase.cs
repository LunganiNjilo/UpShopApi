using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using UpShopApi.Application.Interfaces;
using UpShopApi.Domain.Models;

namespace UpShopTests.Infrastructure
{
    public abstract class TestBase : IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient HttpClient { get; }

        // Expose the mock so tests can configure it in Arrange
        // Replace IRepository with whatever repository interface you actually use (IGenericRepository<Product> etc.)
        protected IRepository<Product> ProductRepositoryMock { get; }

        protected TestBase()
        {
            // create substitute once (tests can re-configure per-test)
            ProductRepositoryMock = Substitute.For<IRepository<Product>>();

            // create factory and register substitutes in DI
            _factory = new CustomWebApplicationFactory<Program>(services =>
            {
                // replace the real registrations with test doubles
                services.AddScoped(_ => ProductRepositoryMock);
                // register any other mocks here if you want defaults
                // services.AddScoped(_ => OtherRepositoryMock);
            });

            HttpClient = _factory.CreateClient();
        }

        /// <summary>
        /// Create a strongly-typed API client wrapper using the shared HttpClient.
        /// Call this from each test's [SetUp].
        /// </summary>
        protected TClient CreateApiClient<TClient>() where TClient : class
        {
            return (TClient)Activator.CreateInstance(typeof(TClient), HttpClient)!;
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
            _factory?.Dispose();
        }
    }

}
