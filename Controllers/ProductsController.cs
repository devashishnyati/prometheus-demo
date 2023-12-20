// ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using PrometheusDemo.Models;
using PrometheusDemo.Services;
using System.Diagnostics;

namespace PrometheusDemo.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductsService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllProducts([FromQuery] GetProductRequestModel requestModel)
        {
            try
            {
                var products = _productService.GetProducts(requestModel);
                var responseModels = products.Select(p => new ProductResponseModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                });

                return Ok(responseModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TraceID: {Activity.Current?.Id} | Error in API | Exception: {ex}");
                return StatusCode(500, new { Message = "Internal Server Error", Details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }

            var responseModel = new ProductResponseModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };

            return Ok(responseModel);
        }

        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductRequestModel requestModel)
        {
            var newProduct = _productService.AddProduct(requestModel);

            var responseModel = new ProductResponseModel
            {
                Id = newProduct.Id,
                Name = newProduct.Name,
                Price = newProduct.Price
            };

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, responseModel);
        }
    }


}