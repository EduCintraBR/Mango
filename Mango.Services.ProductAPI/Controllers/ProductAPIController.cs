using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public ProductAPIController(AppDbContext dbContext, IMapper mapper)
        {
            _db = dbContext;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                _response.Result = _mapper.Map<List<ProductDto>>(_db.Products.ToList());
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                _response.Result = _mapper.Map<ProductDto>(_db.Products.First(a => a.ProductId == id));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post(ProductDto productDto)
        {
            try
            {
                Product productAdded = _mapper.Map<Product>(productDto);
                _db.Products.Add(productAdded);
                _db.SaveChanges();

                if (productDto.Image != null)
                {
                    string fileName = productAdded.ProductId + Path.GetExtension(productDto.Image.FileName);
                    string filePath = $@"wwwroot\ProductImages\{fileName}";
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    productAdded.ImageUrl = $@"{baseUrl}/ProductImages/{fileName}";
                    productAdded.ImageLocalPathUrl = filePath;
                }
                else
                {
                    productAdded.ImageUrl = "https://placehold.co/600x400";
                }

                _db.Products.Update(productAdded);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(productAdded);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                var productAdded = _db.Products.Update(_mapper.Map<Product>(productDto));
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(productAdded.Entity);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                var product = _db.Products.First(c => c.ProductId == id);

                if (!string.IsNullOrEmpty(product.ImageLocalPathUrl))
                {
                    var oldPathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPathUrl);
                    FileInfo file = new FileInfo(oldPathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                _db.Products.Remove(product);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
    }
}
