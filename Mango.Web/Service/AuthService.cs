using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        public readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignToRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                 ApiType = ApiType.POST,
                 Data = registrationRequestDto,
                 Url = $"{AuthAPIBase}/api/auth",
                 AccessToken = ""
            });
        }

        public Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
