using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<OrderHeaderDto> list;
            string userId = string.Empty;

            if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDto response = await _orderService.GetAllOrder(userId);
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDto>>(Convert.ToString(response.Result));
            }
            else
            {
                list = new List<OrderHeaderDto>();
            }

            return Json(new { data = list });
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            OrderHeaderDto orderHeader = new();
            ResponseDto response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            }
            else
            {
                orderHeader = new OrderHeaderDto();
            }

            return View(orderHeader);
        }
    }
}
