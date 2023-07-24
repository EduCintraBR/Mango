using Azure;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ResponseDto _response;

        public UserController(IUserService userService)
        {
            _userService = userService;
            _response = new ResponseDto();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserDto userDto)
        {
            var result = await _userService.CreateUserAsync(userDto);

            if (!result)
            {
                _response.IsSuccess = false;
                _response.Message = "Failed to create an user.";

                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}
