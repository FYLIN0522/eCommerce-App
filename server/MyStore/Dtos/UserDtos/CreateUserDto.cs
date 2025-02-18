using MyStore.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Dtos.UserDtos
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }


        [Required]
        
        public Role Role { get; set; }
    }
}
