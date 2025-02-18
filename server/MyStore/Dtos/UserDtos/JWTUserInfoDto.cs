using System.Security.Claims;

namespace MyStore.Dtos.UserDtos
{
    public class JWTUserInfoDto
    {
        //public ClaimsPrincipal? Claim{ get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
