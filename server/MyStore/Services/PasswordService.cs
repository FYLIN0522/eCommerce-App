using Microsoft.AspNetCore.Identity;
using MyStore.Service;
using Repositories.Abstract;
using static System.Net.Mime.MediaTypeNames;

namespace MyStore.Services
{
    public class PasswordService
    {
        //private readonly PasswordHasher<string> _passwordHasher;
        

        public string HashPassword(String passwordPlainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwordPlainText);
        } 

        public bool Verify(String passwordPlainText, String passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(passwordPlainText, passwordHash);
        }

        public bool Compare(String password, String reqPassword)
        {
            return password == reqPassword;
        }
    }
}
