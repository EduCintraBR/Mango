using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.Dto
{
    public class CreateUserDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
