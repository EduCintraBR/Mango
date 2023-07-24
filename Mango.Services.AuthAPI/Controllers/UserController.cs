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
    //[Authorize(Roles = "ADMIN")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ResponseDto _response;

        public UserController(IUserService userService)
        {
            _userService = userService;
            _response = new ResponseDto();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);

            if (result is null)
            {
                _response.IsSuccess = false;
                _response.Message = "Failed to retrieve the user from database.";

                return BadRequest(_response);
            }

            _response.Result = result;
            return Ok(_response);
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();

            if (!result.Any())
            {
                _response.IsSuccess = false;
                _response.Message = "Failed to retrieve users from database.";

                return BadRequest(_response);
            }

            _response.Result = result;
            return Ok(_response);
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
