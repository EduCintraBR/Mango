using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.RabbitMQSender;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Authorize]
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        //private readonly IAzureMessageBus _cartMessageSender;
        private readonly IRabbitMQCartMessageSender _cartMessageSender;
        private readonly IConfiguration _configuration;

        public CartAPIController(AppDbContext dbContext, IMapper mapper, 
            IProductService productService, ICouponService couponService,
            IRabbitMQCartMessageSender cartMessageSender, IConfiguration configuration)
        {
            _db = dbContext;
            _mapper = mapper;
            _response = new ResponseDto();
            _productService = productService;
            _couponService = couponService;
            _cartMessageSender = cartMessageSender;
            _configuration = configuration;
        }

        [HttpGet("retrieve-products")]
        public async Task<ResponseDto> GetProducts()
        {
            try
            {
                IEnumerable<ProductDto> products = await _productService.GetAllProducts();

                _response.Result = products;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpGet("getCouponCode/{couponCode}")]
        public async Task<ResponseDto> GetCoupon(string couponCode)
        {
            try
            {
                var coupon = await _couponService.GetCoupon(couponCode);

                _response.Result = coupon;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpGet("getCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new CartDto()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(await _db.CartHeaders.FirstAsync(a => a.UserId.Equals(userId)))
                };

                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(a => a.CartHeaderId == cartDto.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> products = await _productService.GetAllProducts();

                cartDto.CartDetails.Join(products, cartDetail => cartDetail.ProductId, product => product.ProductId, (cartDetail, product) =>
                {
                    cartDetail.Product = product;
                    return cartDetail;
                })
                    .ToList();

                cartDto.CartHeader.CartTotal = cartDto.CartDetails.Sum(a => a.Count * a.Product.Price);

                await RecalculateTotalOrderWithCoupon(cartDto);

                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        private async Task RecalculateTotalOrderWithCoupon(CartDto cartDto)
        {
            if (cartDto.CartHeader.Discount == 0 && !string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                CouponDto coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                if (coupon != null && cartDto.CartHeader.CartTotal >= coupon.MinAmount)
                {
                    cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                    cartDto.CartHeader.Discount = coupon.DiscountAmount;
                }
            }
        }

        [HttpPost("applyCoupon")]
        public async Task<ResponseDto> InsertCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if (coupon == null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Este cupom é inválido.";

                        return _response;
                    }
                    else
                    {
                        if(cartDto.CartHeader.CartTotal < coupon.MinAmount)
                        {
                            _response.IsSuccess = false;
                            _response.Message = $"Não é possível aplicar este cupom. O mínimo da compra deve ser {string.Format("{0:c}", coupon.MinAmount)}";

                            return _response;
                        }
                    }
                }

                var cartFromDb = await _db.CartHeaders.FirstAsync(a => a.CartHeaderId == cartDto.CartHeader.CartHeaderId);
                cartFromDb.CouponCode = cartDto.CartHeader?.CouponCode.ToUpper();

                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost("emailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                string nameQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
                _cartMessageSender.SendMessage(cartDto, nameQueue);

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost("removeCoupon")]
        public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(a => a.CartHeaderId == cartDto.CartHeader.CartHeaderId);
                cartFromDb.CouponCode = "";

                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpPost("cartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                if(cartDto.CartDetails.Any(prod => prod.Count > 0))
                {
                    var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId.Equals(cartDto.CartHeader.UserId));
                    if (cartHeaderFromDb == null)
                    {
                        CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                        await _db.CartHeaders.AddAsync(cartHeader);
                        await _db.SaveChangesAsync();

                        cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                        await _db.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        // check if details has same product
                        var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                            u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                            u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                        if (cartDetailsFromDb == null)
                        {
                            cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                            await _db.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                            cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                            cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;

                            _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                            await _db.SaveChangesAsync();
                        }
                    }

                    _response.Result = cartDto;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Insira um ou mais na quantidade do produto";
                }
                
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
                
                int totalCountOfCartItems = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if(totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }

            return _response;
        }
    }
}
