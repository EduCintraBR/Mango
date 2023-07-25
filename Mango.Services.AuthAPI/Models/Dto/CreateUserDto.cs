namespace Mango.Services.AuthAPI.Models.Dto
{
    public class CreateUserDto
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? RoleName { get; set; }
    }
}
