using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public CouponAPIController(AppDbContext dbContext, IMapper mapper)
        {
            _db = dbContext;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                _response.Result = _mapper.Map<List<CouponDto>>(_db.Coupons.ToList());
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                _response.Result = _mapper.Map<CouponDto>(_db.Coupons.First(a => a.CouponId == id));
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize]
        [HttpGet("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                var couponFromDb = _db.Coupons.FirstOrDefault(a => a.CouponCode.Equals(code.ToLower()));

                _response.Result = _mapper.Map<CouponDto>(couponFromDb);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                var couponAdded = _db.Coupons.Add(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();

                Stripe.StripeConfiguration.ApiKey = "sk_test_51NaN35DcqWkCvyrCmtMGzwYQ7JqDc6tJkyR0Tkf2KdhTmD0axnu8qbRnOmmgxDKqgHze3VcrMBXjPYvyvvVNJ3Q000ANPgGgt5";

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDto.DiscountAmount * 100),
                    Name = couponDto.CouponCode,
                    Currency = "brl",
                    Id = couponDto.CouponCode
                };
                var service = new Stripe.CouponService();
                service.Create(options);

                _response.Result = _mapper.Map<CouponDto>(couponAdded.Entity);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                var couponAdded = _db.Coupons.Update(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(couponAdded.Entity);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                var coupon = _db.Coupons.First(c => c.CouponId == id);

                _db.Coupons.Remove(coupon);
                _db.SaveChanges();

                var service = new Stripe.CouponService();
                service.Delete(coupon.CouponCode);
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
