using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class UserService : IUserService
    {
        private readonly IBaseService _baseService;
        private readonly ITokenProvider _tokenProvider;

        public UserService(IBaseService baseService, ITokenProvider tokenProvider)
        {
            _baseService = baseService;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto?> GetAllUsersAsync()
        {
            var token = _tokenProvider.GetToken();
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = $"{AuthAPIBase}/api/user/all-users",
                AccessToken = token
            }, withBearer: false);
        }

        public async Task<ResponseDto?> GetUserByIdAsync(Guid userId)
        {
            var token = _tokenProvider.GetToken();
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = $"{AuthAPIBase}/api/user/{userId}",
                AccessToken = token
            }, withBearer: false);
        }

        public async Task<ResponseDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            var token = _tokenProvider.GetToken();
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = createUserDto,
                Url = $"{AuthAPIBase}/api/user/create",
                AccessToken = token
            }, withBearer: false);
        }

        public async Task<ResponseDto?> DeleteUserAsync(Guid userId)
        {
            var token = _tokenProvider.GetToken();
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.DELETE,
                Url = $"{AuthAPIBase}/api/user/delete/{userId}",
                AccessToken = token
            }, withBearer: false);
        }

        public async Task<ResponseDto?> UpdateUserAsync(UpdateUserDto createUserDto)
        {
            var token = _tokenProvider.GetToken();
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.PUT,
                Data = createUserDto,
                Url = $"{AuthAPIBase}/api/user/update",
                AccessToken = token
            }, withBearer: false);
        }
    }
}
