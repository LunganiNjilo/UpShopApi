using Microsoft.AspNetCore.Mvc;
using UpShopApi.Application.Interfaces;
using UpShopApi.Domain.Models;

namespace UpShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET api/products
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var product = await _productService.GetAllProductsAsync();
            return Ok(product);
        }

        // GET api/products/{id}
        [HttpGet("{sku}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductBySkuAsync(string sku)
        {
            var product = await _productService.GetProductBySkuAsync(sku);
            return Ok(product);
        }
    }
}
