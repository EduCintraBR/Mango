using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Service.IService
{
    public interface IUserService
    {
        Task<ResponseDto?> RegisterAsync(CreateUserDto createUserDto);
    }
}
