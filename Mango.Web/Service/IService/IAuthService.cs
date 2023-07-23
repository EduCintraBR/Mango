using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> AssignToRoleAsync(RegistrationRequestDto registrationRequestDto);
    }
}
