using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
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

        [HttpPost]
        public async Task<IActionResult> UserCreate(CreateUserDto userDto)
        {
            ResponseDto? response = await _userService.CreateUserAsync(userDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Usuário criado com sucesso!";
                return RedirectToAction(nameof(UserIndex));
            }
            else
            {
                TempData["error"] = response.Message;
            }

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

        [HttpPost]
        public async Task<IActionResult> UserUpdate(UserDto userDto)
        {
            var userToUpdate = new UpdateUserDto()
            {
                Id = userDto.Id,
                Email = userDto.Email,
                Name = userDto.Name,
                PhoneNumber = userDto.PhoneNumber
            };

            ResponseDto? response = await _userService.UpdateUserAsync(userToUpdate);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Usuário alterado com sucesso!";
                return RedirectToAction(nameof(UserIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
                return View(userDto);
            }
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

        [HttpPost]
        public async Task<IActionResult> UserDelete(UserDto userDto)
        {
            ResponseDto? response = await _userService.DeleteUserAsync(userDto.Id);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Usuário deletado com sucesso!";
                return RedirectToAction(nameof(UserIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
                return View();
            }
        }
    }
}
