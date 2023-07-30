using Mango.Web.Models;
using Mango.Web.Models.Dto;
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

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedUser());
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
