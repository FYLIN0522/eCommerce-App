using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyStore.Dtos.UserDtos;
using MyStore.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyStore.Service
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
            //_httpContextAccessor = httpContextAccessor;
        }

        public JWTUserInfoDto GetJWTUserInfo(ClaimsPrincipal userClaim)
        {
            JWTUserInfoDto details = new JWTUserInfoDto();
            if (userClaim is not null)
            {
                details.UserId = userClaim.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                details.Email = userClaim.FindFirst(ClaimTypes.Email)?.Value;
                details.Role = userClaim.FindFirst(ClaimTypes.Role)?.Value;
                return details;
            }

            return details;
        }

        public string GenerateRefreshToken()
        {
            var refreshTokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(refreshTokenBytes);
            }

            return Convert.ToBase64String(refreshTokenBytes);
        }

        public string GenUserToken(int id, string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
            };

            try
            {
                var token = _configuration.GetSection("JWT");
                var accessExpiration = double.Parse(token["accessExpiration"]);
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token["secret"]?? throw new NullReferenceException("Secret is not configured.")));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var jwtToken = new JwtSecurityToken(
                                        issuer: token["issuer"],
                                        null,
                                        claims,
                                        expires: DateTime.UtcNow.AddMinutes(accessExpiration),
                                        signingCredentials: credentials);

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return accessToken;
            }
            catch (Exception err)
            {
                Console.WriteLine($"An error occurred: {err.Message}");
                throw;
            }
            //if (tokenParameter["accessExpiration"] is not null
            //    && tokenParameter["secret"] is not null
            //    && tokenParameter["issuer"] is not null) 
            //{
        }
    }
}
