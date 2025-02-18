using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.UserDtos
{
    public class UpdateUserDto
    {

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MinLength(6)]
        public string? NewPassword { get; set; }

        [MinLength(6)]
        public string? CurrentPassword { get; set; }
    }
}
