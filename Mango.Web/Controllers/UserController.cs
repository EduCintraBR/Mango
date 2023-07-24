using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> UserIndex()
        {
            List<UserDto>? list = new();
            ResponseDto? response = await _userService.GetAllUsersAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<UserDto>?>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        public async Task<IActionResult> UserCreate()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{ Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem{ Text = SD.RoleCustomer, Value = SD.RoleCustomer },
            };

            ViewBag.RoleList = roleList;

            return View();
        }

        public async Task<IActionResult> UserUpdate(string userId)
        {
            UserDto userDto = new();
            ResponseDto? response = await _userService.GetUserByIdAsync(Guid.Parse(userId));

            if (response != null && response.IsSuccess)
            {
                userDto = JsonConvert.DeserializeObject<UserDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(userDto);
        }

        public async Task<IActionResult> UserDelete(string userId)
        {
            UserDto userDto = new();
            ResponseDto? response = await _userService.GetUserByIdAsync(Guid.Parse(userId));

            if (response != null && response.IsSuccess)
            {
                userDto = JsonConvert.DeserializeObject<UserDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{ Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem{ Text = SD.RoleCustomer, Value = SD.RoleCustomer },
            };

            ViewBag.RoleList = roleList;

            return View(userDto);
        }
    }
}
