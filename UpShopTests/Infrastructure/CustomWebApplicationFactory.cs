using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace UpShopTests.Infrastructure
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection> _configureTestServices;

        public CustomWebApplicationFactory(Action<IServiceCollection> configureTestServices = null)
        {
            _configureTestServices = configureTestServices;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                _configureTestServices?.Invoke(services);
            });
        }
    }
}
