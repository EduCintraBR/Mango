using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class UserService : IUserService
    {
        private readonly IBaseService _baseService;

        public UserService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllUsersAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = $"{AuthAPIBase}/api/user/all-users",
            });
        }

        public async Task<ResponseDto?> GetUserByIdAsync(Guid userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = $"{AuthAPIBase}/api/user/{userId}",
            });
        }

        public async Task<ResponseDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = createUserDto,
                Url = $"{AuthAPIBase}/api/user/create",
            });
        }

        public async Task<ResponseDto?> DeleteUserAsync(Guid userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.DELETE,
                Url = $"{AuthAPIBase}/api/user/delete/{userId}",
            });
        }

        public async Task<ResponseDto?> UpdateUserAsync(UpdateUserDto createUserDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.PUT,
                Data = createUserDto,
                Url = $"{AuthAPIBase}/api/user/update",
            });
        }
    }
}
