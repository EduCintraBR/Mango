﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.Dto
{
    public class CreateUserDto
    {
        [Key]
        public Guid? Id { get; set; }
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
