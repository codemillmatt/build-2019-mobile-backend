using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TailwindTraders.Mobile.Features.Product;
using MonkeyCache.SQLite;
using TailwindTraders.Mobile.Framework;
using Xamarin.Forms;
using Refit;
using TailwindTraders.Mobile.Features.Common;
using TailwindTraders.Mobile;
using System.Linq;
using TailwindTraders.Mobile.Helpers;

[assembly: Dependency(typeof(ProductService))]
namespace TailwindTraders.Mobile
{
    public class ProductService : IProductService
    {
        readonly IConnectivityService connectivityService;
        readonly IProductsWebAPI webAPI;
        readonly string allProductsKey = "allProducts";

        string url = "** YOUR PRODUCT URL HERE - up through the api route portion - refit in IProductsWebAPI appends the rest **";
        // readonly string productsBaseUrl = "http://localhost:8000/api";

        public ProductService()
        {
            connectivityService = DependencyService.Get<IConnectivityService>();

            var productsRefitSettings = new RefitSettings { ContentSerializer = new JsonContentSerializer(ProductPerDTOJsonConverter.Settings) };

            webAPI = RestService.For<IProductsWebAPI>(
                HttpClientFactory.Create(productsBaseUrl), productsRefitSettings
            );
        }

        public async Task<ProductsPerTypeDTO> GetProductsAsync()
        {
            if (!connectivityService.IsThereInternet && Barrel.Current.Exists(allProductsKey))
            {
                return Barrel.Current.Get<ProductsPerTypeDTO>(allProductsKey);
            }

            if (!Barrel.Current.IsExpired(allProductsKey) && Barrel.Current.Exists(allProductsKey))
            {
                return Barrel.Current.Get<ProductsPerTypeDTO>(allProductsKey);
            }

            var allProducts = await webAPI.GetProductsAsync();

            Barrel.Current.Add(allProductsKey, allProducts, TimeSpan.FromMinutes(60));

            return allProducts;
        }

        public async Task<ProductDTO> GetProductDetailAsync(long productId)
        {
            // Pull from the all products
            var allProducts = await GetProductsAsync();

            return allProducts.Products.First(p => p.ItemId == productId);
        }

        public async Task<ProductsPerTypeDTO> GetProductsPerCategory(string category)
        {
            var allProducts = await GetProductsAsync();

            var productsByCategory = new ProductsPerTypeDTO
            {
                Products = allProducts.Products.Where(p => p.ProductType.ToString() == category).ToList()
            };

            productsByCategory.Size = productsByCategory.Products.Count();

            return productsByCategory;
        }
    }

    public interface IProductsWebAPI
    {
        [Get("/products/{id}")]
        Task<ProductDTO> GetDetailAsync(long id);

        [Get("/products")]
        Task<ProductsPerTypeDTO> GetProductsAsync();

        [Get("/products/category/{category}")]
        Task<ProductsPerTypeDTO> GetProductsByCategoryAsync(string category);
    }
}
