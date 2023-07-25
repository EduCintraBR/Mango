namespace Mango.Web.Models.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name{ get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
