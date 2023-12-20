using PrometheusDemo.Models;
using System.Collections;
using System.Collections.Generic;
using PrometheusDemo.MetricsConfig;
using Prometheus;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PrometheusDemo.Services
{
    public class ProductsService
    {
        private readonly ILogger<ProductsService> _logger;
        private readonly List<ProductResponseModel> _products = new List<ProductResponseModel>
    {
        new ProductResponseModel { Id = 1, Name = "Product 1", Price = 19.99m },
        new ProductResponseModel { Id = 2, Name = "Product 2", Price = 29.99m },
        // Add more products as needed
    };

        public ProductsService(ILogger<ProductsService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<ProductResponseModel> GetProducts(GetProductRequestModel requestModel)
        {
            List<ProductResponseModel> result = new List<ProductResponseModel>();
            using (ProductsMetrics.GetBSEStarMFProductsDuration.NewTimer())
            {
                try
                {
                    List<ProductResponseModel> BseProducts = new List<ProductResponseModel>();
                    BseProducts = GetProductsFromBSEStarMF(requestModel.BSESuccessFlag);
                    result.AddRange(BseProducts);
                    Thread.Sleep(1000);
                    ProductsMetrics.GetBSEStarMFProductsSuccess.Inc();
                }

                catch (Exception ex)
                {
                    ProductsMetrics.GetBSEStarMFProductsError.Inc();
                    _logger.LogError($"TraceID: {Activity.Current?.Id} | Error in GetBSEStarMFProducts | Exception: {ex}");
                }
            }

            using (ProductsMetrics.GetInvestwellProductsDuration.NewTimer())
            {
                try
                {
                    List<ProductResponseModel> InvestwellProducts = new List<ProductResponseModel>();
                    InvestwellProducts = GetProductsFromInvestWell(requestModel.InvestWellSuccessFlag);
                    result.AddRange(InvestwellProducts);
                    Thread.Sleep(500);
                    ProductsMetrics.GetInvestwellProductsSuccess.Inc();
                }

                catch (Exception ex)
                {
                    ProductsMetrics.GetInvestwellProductsError.Inc();
                    _logger.LogError($"TraceID: {Activity.Current?.Id} | Error in GetProductsFromInvestWell | Exception: {ex}");
                }
            }
            // DB Logic
            if (!requestModel.ErrorFlag) {
                ProductsMetrics.GetProductsDBError.WithLabels(requestModel.DbSource).Inc();
                _logger.LogError($"TraceID: {Activity.Current?.Id} | Error in DB");
                throw new Exception();
            }
            
            ProductsMetrics.GetProductsDBSuccess.WithLabels(requestModel.DbSource).Inc();
            result.AddRange(_products.Where(p => p.Id >= 3));
            
            return result;
        }

        public ProductResponseModel GetProductById(int id)
        {
            return _products.Find(p => p.Id == id);
        }

        public ProductResponseModel AddProduct(ProductRequestModel requestModel)
        {
            var newProduct = new ProductResponseModel
            {
                Id = _products.Count + 1, // In a real-world scenario, generate a unique ID
                Name = requestModel.Name,
                Price = requestModel.Price
            };

            _products.Add(newProduct);
            return newProduct;
        }

        private List<ProductResponseModel> GetProductsFromBSEStarMF(bool BSESuccessFlag)
        {
            if (!BSESuccessFlag)
            {
                throw new Exception();
            }
            List<ProductResponseModel> result = new List<ProductResponseModel>();
            result.Add(_products[0]);
            return result;

        }

        private List<ProductResponseModel> GetProductsFromInvestWell(bool InvestWellSuccessFlag)
        {

            if (!InvestWellSuccessFlag)
            {
                throw new Exception();
            }
            List<ProductResponseModel> result = new List<ProductResponseModel>();
            result.Add(_products[1]);
            return result;

        }
    }
}
