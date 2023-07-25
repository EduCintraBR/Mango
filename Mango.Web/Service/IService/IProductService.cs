using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDto?> GetAllProductsAsync();
        Task<ResponseDto?> GetProductByIdAsync(int ProductId);
        Task<ResponseDto?> CreateProductAsync(ProductDto ProductDto);
        Task<ResponseDto?> UpdateProductAsync(ProductDto ProductDto);
        Task<ResponseDto?> DeleteProductAsync(int ProductId);
    }
}
