using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;

        public UserService(AppDbContext database, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _db = database;
            _userManager = userManager;
            _authService = authService;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            return await _db.ApplicationUsers.Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            })
                .FirstOrDefaultAsync(u => u.Id.Equals(userId.ToString()));
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _db.ApplicationUsers.Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            })
                .ToListAsync();
        }

        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto)
        {
            ApplicationUser user = new()
            {
                UserName = createUserDto.Email,
                Email = createUserDto.Email,
                NormalizedEmail = createUserDto.Email.ToUpper(),
                Name = createUserDto.Name,
                PhoneNumber = createUserDto.PhoneNumber
            };

            var userAdded = await _userManager.CreateAsync(user, createUserDto.Password);
            
            if (userAdded != null && userAdded.Succeeded)
            {
                var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.Equals(user.UserName));
                if (userFromDb != null)
                {
                    var roleAssigned = await _authService.AssignRole(userFromDb.UserName, createUserDto.RoleName);
                    
                    if (roleAssigned) return true;
                }
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id.Equals(userId.ToString()));
            if (userFromDb != null)
            {
                _db.ApplicationUsers.Remove(userFromDb);
                await _db.SaveChangesAsync();
                
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id.Equals(updateUserDto.Id.ToString()));
            if (userFromDb != null)
            {
                userFromDb.Name = updateUserDto.Name;
                userFromDb.UserName = updateUserDto.Email;
                userFromDb.Email = updateUserDto.Email;
                userFromDb.PhoneNumber = updateUserDto.PhoneNumber;

                _db.ApplicationUsers.Update(userFromDb);
                await _db.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
