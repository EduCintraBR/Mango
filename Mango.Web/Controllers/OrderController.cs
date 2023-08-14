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

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status do pedido atualizado!";
                return RedirectToAction(nameof(OrderDetails), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status do pedido atualizado!";
                return RedirectToAction(nameof(OrderDetails), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status do pedido atualizado!";
                return RedirectToAction(nameof(OrderDetails), new { orderId = orderId });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
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
                
                switch (status)
                {
                    case "approved":
                        list = list.Where(l => l.Status.Equals(SD.Status_Approved));
                        break;
                    case "readyforpickup":
                        list = list.Where(l => l.Status.Equals(SD.Status_ReadyForPickup));
                        break;
                    case "cancelled":
                        list = list.Where(l => l.Status.Equals(SD.Status_Cancelled) || l.Status.Equals(SD.Status_Refunded));
                        break;
                    default:
                        break;
                }
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
