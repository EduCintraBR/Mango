﻿using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<bool> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}
