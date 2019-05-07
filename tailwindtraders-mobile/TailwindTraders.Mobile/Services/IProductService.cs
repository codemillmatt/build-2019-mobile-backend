using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TailwindTraders.Mobile.Features.Product;


namespace TailwindTraders.Mobile
{
    public interface IProductService
    {
        Task<ProductsPerTypeDTO> GetProductsAsync();
        Task<ProductDTO> GetProductDetailAsync(long productId);
        Task<ProductsPerTypeDTO> GetProductsPerCategory(string category);
    }
}
