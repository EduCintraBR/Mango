using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{ProductAPIBase}/api/product",
                Data = ProductDto,
                ContentType = ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ProductAPIBase}/api/product"
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int ProductId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ProductAPIBase}/api/product/{ProductId}"
            });
        }

        public async Task<ResponseDto?> GetProductAsync(string ProductCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ProductAPIBase}/api/product/GetByCode/{ProductCode}"
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Url = $"{ProductAPIBase}/api/product",
                Data = ProductDto,
                ContentType = ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int ProductId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = $"{ProductAPIBase}/api/product/{ProductId}"
            });
        }
    }
}
