using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public CouponService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<CouponDto?> GetCoupon(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            
            //HttpRequestMessage message = new();
            //message.Headers.Add("Accept", "application/json");

            //var token = _tokenProvider.GetToken();
            //message.Headers.Add("Authorization", $"Bearer {token}");

            var apiResponse = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(Convert.ToString(apiContent));

            if (apiResponseDto.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(apiResponseDto.Result));
            }

            return null;
        }
    }
}
