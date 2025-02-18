using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Dtos.UserDtos;
using MyStore.Models;
using Repositories.Abstract;

namespace MyStore.Repositories.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreContext _context;
        public UserRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task CreateUserAsync(User userDetails)
        {
            _context.Users.Add(userDetails);
            await _context.SaveChangesAsync();
        }


        public async Task<ResponseUserDto?> GetUserById(int id)
        {

            return await _context.Users
                            .AsNoTracking()
                            .Select(u => new ResponseUserDto
                            {
                                UserId = u.UserId,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Email = u.Email,
                                Role = u.Role
                            })
                            .SingleOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetUserEntityById(int id)
        {

            return await _context.Users.SingleOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetUserEntityByIdNoTrack(int id)
        {

            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailForUpdate(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailNoTrack(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshToken(User user, string token, DateTime time)
        {

            user.RefreshToken = token;
            user.RefreshTokenExpTime = time;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRefreshToken(User user)
        {

            user.RefreshToken = null;
            user.RefreshTokenExpTime = null;
            await _context.SaveChangesAsync();
        }
        //public async Task<bool> ComparePasswordAsync(string userPassword, string password)
        //{
        //    return await userPassword == userPassword;
        //    // Need to decode userPassword
        //}
    }
}
