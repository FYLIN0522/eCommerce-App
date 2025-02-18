using MyStore.Models;
using MyStore.Dtos.UserDtos;

namespace Repositories.Abstract
{
    public interface IUserRepository
    {
        Task CreateUserAsync(User userDeatils);
        Task<ResponseUserDto?> GetUserById(int id);
        Task<User?> GetUserEntityById(int id);
        Task<User?> GetUserEntityByIdNoTrack(int id);
        Task<User?> GetUserByEmailForUpdate(string email);
        Task<User?> GetUserByEmailNoTrack(string email);
        Task<bool> EmailExistsAsync(string email);
        Task UpdateUser(User user);

        Task UpdateRefreshToken(User user, string token, DateTime time);
        Task DeleteRefreshToken(User user);
        //Task<bool> ComparePasswordAsync(string userPassword, string entryPassword);
    }
}
