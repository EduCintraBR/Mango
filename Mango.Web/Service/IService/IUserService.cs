using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Service.IService
{
    public interface IUserService
    {
        Task<ResponseDto?> GetUserByIdAsync(Guid userId);
        Task<ResponseDto?> GetAllUsersAsync();
        Task<ResponseDto?> CreateUserAsync(CreateUserDto createUserDto);
        Task<ResponseDto?> UpdateUserAsync(UpdateUserDto createUserDto);
        Task<ResponseDto?> DeleteUserAsync(Guid userId);
    }
}
