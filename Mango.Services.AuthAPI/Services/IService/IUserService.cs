using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(CreateUserDto userDto);
        Task<bool> UpdateUser(ApplicationUser user);
        Task<bool> DeleteUser(ApplicationUser user);
    }
}
