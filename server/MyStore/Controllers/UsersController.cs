using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using MyStore.Models;
using MyStore.Service;
using Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyStore.Dtos.UserDtos;
using Microsoft.AspNetCore.Identity;
using MyStore.Services;
using System.Text.RegularExpressions;
using System;

namespace MyStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly JWTService _service;
        private readonly PasswordService _passwordservice;
        //private readonly PasswordHasher<string> _passwordHasher;

        public UsersController(IUserRepository repository, JWTService service, IConfiguration configuration, PasswordService passwordservice)
        {
            _repository = repository;
            _service = service;
            _configuration = configuration;
            //_passwordHasher = new PasswordHasher<string>();
            _passwordservice = passwordservice;
        }


        // POST: api//users/register  Register as a new user
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(CreateUserDto userDetails)
        {
            //StatusCode: 403 
            bool emailExists = await _repository.EmailExistsAsync(userDetails.Email);
            if (emailExists)
            {
                return StatusCode(403, "Email already in use.");
            }
            var passwordHash = _passwordservice.HashPassword(userDetails.Password);

            User registerReq = new User
            {
                Email = userDetails.Email,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Password = passwordHash,
                Role = userDetails.Role
            };
            
            

            await _repository.CreateUserAsync(registerReq);

            ResponseUserDto res = new ResponseUserDto
            {
                UserId = registerReq.UserId,
                Email = userDetails.Email,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Role = userDetails.Role
            };
            return CreatedAtAction(nameof(GetUser), new { id = registerReq.UserId }, res);
        }


        // GET: api/Users/<id>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseUserDto>> GetUser(int id)
        {
            var user = HttpContext.User;
            //var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var email = user.FindFirst(ClaimTypes.Email)?.Value;
            //var role = user.FindFirst(ClaimTypes.Role)?.Value;
            JWTUserInfoDto userInfo = _service.GetJWTUserInfo(user);
            var userDetails = await _repository.GetUserEntityByIdNoTrack(id);
            if (userDetails == null)
            {
                return NotFound();
            }

            ResponseUserDto responseUser = new ResponseUserDto
                                                {
                                                    Email = userDetails.Email,
                                                    UserId = userDetails.UserId,
                                                    FirstName = userDetails.FirstName,
                                                    LastName = userDetails.LastName,
                                                    Role = userDetails.Role
                                                };

            bool tokenIsVaild = user?.Identity?.IsAuthenticated ?? false;
            // The email field is only returned when the currently authenticated user is viewing their own details.
            if (userInfo.Role == "Admin")
            {
                return Ok(responseUser);
            }

            if (!tokenIsVaild
                || userDetails.RefreshToken is null // Checking RefreshToken
                || int.Parse(userInfo.UserId) != id
                || userInfo.UserId is null
                || userInfo.Email is null
                || userInfo.Role is null)
            {
                responseUser.Email = "**********";
                return Ok(responseUser);

            }
            else
            {
                return Ok(responseUser);
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<RefreshTokenDto>> Login([FromBody] LoginRequestDto request)
        {
            if (request.email == null || request.password == null)
            {
                return BadRequest("Invalid Request");
            }

            // User Entity
            var userDetails = await _repository.GetUserByEmailForUpdate(request.email);

            if (userDetails is null)
            {
                return StatusCode(401, "UnAuthorized. this email is unregistered.");
            }

            var correct = _passwordservice.Verify(request.password, userDetails.Password);

            if (!correct)
            {
                return StatusCode(401, "UnAuthorized. Incorrect email/password.");
            }

            string accessToken = _service.GenUserToken(userDetails.UserId, userDetails.Email, userDetails.Role.ToString());
            string refreshToken = _service.GenerateRefreshToken();
            DateTime refreshTokenExpTime = DateTime.Now.AddMinutes(60);
            await _repository.UpdateRefreshToken(userDetails, refreshToken, refreshTokenExpTime);

            RefreshTokenDto tokens = new RefreshTokenDto
            {
                accessToken = accessToken,
                refreshToken = refreshToken,
            };

            return Ok(tokens);
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto token)
        {
            var user = HttpContext.User;
            //var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var email = user.FindFirst(ClaimTypes.Email)?.Value;
            //var role = user.FindFirst(ClaimTypes.Role)?.Value;

            JWTUserInfoDto userInfo = _service.GetJWTUserInfo(user);
            var userDetails = await _repository.GetUserEntityById(int.Parse(userInfo.UserId));
            if (userDetails is null || userDetails.RefreshToken is null)
            {
                return StatusCode(401, "Unauthorized. Cannot log out if you are not authenticated.");
            }

            if (userDetails.RefreshToken != token.refreshToken)
            {
                return Ok("[Ok] Logout");
            } else {
                await _repository.DeleteRefreshToken(userDetails);
                return Ok("[Ok] Logout");
            }
            //if (userDetails == null)
            //{
            //    return NotFound();
            //}
            //var userDetails = await _repository.GetUserEntityById(id);
        }


        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateUserDto putUserReq)
        {

            try
            {
                var user = HttpContext.User;
                //var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //var email = user.FindFirst(ClaimTypes.Email)?.Value;
                //var role = user.FindFirst(ClaimTypes.Role)?.Value;
                JWTUserInfoDto userInfo = _service.GetJWTUserInfo(user);

                var userEntity = await _repository.GetUserEntityById(id);

                if (userEntity is null)
                {
                    return NotFound("Not Found. No product found with id");
                }
                if (int.Parse(userInfo.UserId) != id)
                {
                    return StatusCode(403, ("Can not edit another user's information"));
                }

                //PUT Request body
                if (putUserReq.Email is not null)
                {
                    //StatusCode: 403 
                    bool nameExists = await _repository.EmailExistsAsync(putUserReq.Email);
                    if (nameExists)
                    {
                        return StatusCode(403, "Email already exists!");
                    }

                    userEntity.Email = putUserReq.Email;
                }

                if (putUserReq.FirstName is not null)
                {
                    userEntity.FirstName = putUserReq.FirstName;
                }
                if (putUserReq.LastName is not null)
                {
                    userEntity.LastName = putUserReq.LastName;
                }

                if (putUserReq.NewPassword is not null) 
                {
                    if (putUserReq.CurrentPassword != null) {
                        var correct = _passwordservice.Verify(putUserReq.CurrentPassword, userEntity.Password);
                        if (!correct) 
                        {
                            return StatusCode(401, "Incorrect currentPassword");
                        } else {
                            correct = _passwordservice.Verify(putUserReq.NewPassword, userEntity.Password);
                            if (correct) 
                            {
                                return BadRequest("New Password is same as Current Password");
                            }
                        }
                        var passwordHash = _passwordservice.HashPassword(putUserReq.NewPassword);
                        userEntity.Password = passwordHash;
                    } else {
                        return BadRequest("currentPassword must be supplied to change password");
                    }
                }

                await _repository.UpdateUser(userEntity);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }



        [HttpPost("refreshToken")]
        public async Task<ActionResult<RefreshTokenDto>> RefreshToken([FromBody] RefreshTokenDto request)
        {
            var token = _configuration.GetSection("JWT");
            if (request.accessToken == null && request.refreshToken == null)
            {
                return BadRequest("Invalid Request");
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal claims = handler.ValidateToken(request.accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token["secret"])),
                    ValidIssuer = token["issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = false,

                }, out SecurityToken securityToken);

                JWTUserInfoDto userInfo = _service.GetJWTUserInfo(claims);
                //var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //var email = claims.FindFirst(ClaimTypes.Email)?.Value;
                //var role = claims.FindFirst(ClaimTypes.Role)?.Value;

                if (userInfo.UserId is null
                    || userInfo.Email is null
                    || userInfo.Role is null)
                {
                    return BadRequest("Access Token is invaild.");
                }

                var userDetails = await _repository.GetUserByEmailForUpdate(userInfo.Email);
                if (userDetails.RefreshToken is null || userDetails.RefreshTokenExpTime <= DateTime.Now)
                {
                    return StatusCode(401, "RefreshToken is expired.");
                }
                if (userDetails.RefreshToken != userDetails.RefreshToken)
                {
                    return StatusCode(401, "Forbidden, you may have logged in on other devices.");
                }
                if (userDetails.UserId != int.Parse(userInfo.UserId))
                {
                    return StatusCode(403, "Forbidden.");
                }

                string accessToken = _service.GenUserToken(int.Parse(userInfo.UserId), userInfo.Email, userInfo.Role);
                string refreshToken = _service.GenerateRefreshToken();
                DateTime refreshTokenExpTime = DateTime.Now.AddDays(7);
                await _repository.UpdateRefreshToken(userDetails, refreshToken, refreshTokenExpTime);

                RefreshTokenDto tokens = new RefreshTokenDto
                {
                    accessToken = accessToken,
                    refreshToken = refreshToken,
                };

                return Ok(tokens);
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Authentication failed: accessToken");
            }
            catch (Exception err)
            {
                return BadRequest($"An error occurred: {err.Message}");
            }
        }

    }
}
