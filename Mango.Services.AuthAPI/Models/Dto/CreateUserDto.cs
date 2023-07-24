namespace Mango.Services.AuthAPI.Models.Dto
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? RoleName { get; set; }
    }
}
