using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{OrderAPIBase}/api/order/createOrder",
                Data = cartDto
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{OrderAPIBase}/api/order/createStripeSession",
                Data = stripeRequestDto
            });
        }

        public async Task<ResponseDto?> GetAllOrder(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return await _baseService.SendAsync(new RequestDto
                {
                    ApiType = ApiType.GET,
                    Url = $"{OrderAPIBase}/api/order/getOrders"
                });
            }
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{OrderAPIBase}/api/order/getOrders?userId={userId}"
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{OrderAPIBase}/api/order/getOrder/{orderId}"
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PATCH,
                Url = $"{OrderAPIBase}/api/order/updateOrderStatus/{orderId}",
                Data = newStatus
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Url = $"{OrderAPIBase}/api/order/validateStripeSession",
                Data = orderHeaderId
            });
        }
    }
}
