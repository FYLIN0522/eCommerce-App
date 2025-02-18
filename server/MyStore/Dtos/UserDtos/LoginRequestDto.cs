using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.UserDtos
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string? email { get; set; }


        [Required]
        [MinLength(6)]
        public string? password { get; set; }
    }
}
