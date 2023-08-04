using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = new OrderHeaderDto();

                CartDto cart = await LoadCartDtoBasedOnLoggedUser();
                cart.CartHeader.Phone = cartDto.CartHeader.Phone;
                cart.CartHeader.Email = cartDto.CartHeader.Email;
                cart.CartHeader.Name = cartDto.CartHeader.Name;

                ResponseDto? response = await _orderService.CreateOrder(cart);

                if (response != null && response.IsSuccess)
                {
                    orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

                    var domain = $"{Request.Scheme}://{Request.Host.Value}/";
                    StripeRequestDto stripeRequestDto = new()
                    {
                        ApprovedUrl = $"{domain}cart/confirmation?orderId={orderHeaderDto.OrderHeaderId}",
                        CancelUrl = $"{domain}cart/checkout",
                        OrderHeader = orderHeaderDto,
                    };

                    var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                    if (response != null || response.IsSuccess)
                    {
                        StripeRequestDto stripeResponseObject = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
                        Response.Headers.Add("Location", stripeResponseObject.StripeSessionUrl);

                        return new StatusCodeResult(303);
                    }
                    else
                    {
                        TempData["error"] = stripeResponse?.Message;
                    }
                }
                else
                {
                    TempData["error"] = response?.Message;
                }

                return View(cart);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex?.Message;
            }

            return View(cartDto);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
        }

        private async Task<CartDto?> LoadCartDtoBasedOnLoggedUser()
        {
            var userId = GetUserIdLogged();
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);

            if (response != null && response.IsSuccess)
            {
                CartDto? cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cart;
            }
            else
            {
                return new CartDto()
                {
                    CartHeader = new CartHeaderDto(),
                    CartDetails = new List<CartDetailsDto>()
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCouponCode(CartDto cartDto)
        {
            ResponseDto apiResponse = await _cartService.ApplyCouponAsync(cartDto);

            if (apiResponse != null && apiResponse.IsSuccess)
            {
                TempData["success"] = "Cupom aplicado com sucesso." ;
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = apiResponse?.Message;
                return RedirectToAction(nameof(CartIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedUser();
            cart.CartHeader.Email = GetUserEmailLogged();

            ResponseDto apiResponse = await _cartService.EmailCart(cart);

            if (apiResponse != null && apiResponse.IsSuccess)
            {
                TempData["success"] = "Email será processado e enviado em breve!";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = apiResponse?.Message;
                return RedirectToAction(nameof(CartIndex));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCouponCode(CartDto cartDto)
        {
            if (cartDto != null)
            {
                cartDto.CartHeader.CouponCode = "";
                ResponseDto apiResponse = await _cartService.ApplyCouponAsync(cartDto);

                if (apiResponse != null && apiResponse.IsSuccess)
                {
                    TempData["success"] = "Cupom removido com sucesso.";
                    return RedirectToAction(nameof(CartIndex));
                }
                TempData["error"] = "Falha ao remover o cupom.";
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveCart(int cartDetailsId)
        {
            ResponseDto apiResponse = await _cartService.RemoveFromCartAsync(cartDetailsId);

            if (apiResponse != null && apiResponse.IsSuccess)
            {
                TempData["success"] = "Carrinho atualizado com sucesso.";
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        private string GetUserIdLogged()
        {
            return User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        }

        private string GetUserEmailLogged()
        {
            return User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
        }
    }
}
