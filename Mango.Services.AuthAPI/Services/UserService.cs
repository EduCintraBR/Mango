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

        public async Task<bool> CreateUserAsync(CreateUserDto userDto)
        {
            ApplicationUser user = new()
            {
                UserName = userDto.Email,
                Email = userDto.Email,
                NormalizedEmail = userDto.Email.ToUpper(),
                Name = userDto.Name,
                PhoneNumber = userDto.PhoneNumber
            };

            var userAdded = await _userManager.CreateAsync(user, userDto.Password);
            
            if (userAdded != null && userAdded.Succeeded)
            {
                var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.Equals(user.UserName));
                if (userFromDb != null)
                {
                    var roleAssigned = await _authService.AssignRole(userFromDb.UserName, userDto.RoleName);
                    
                    if (roleAssigned) return true;
                }
                return true;
            }
            return false;
        }

        public Task<bool> DeleteUser(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUser(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
