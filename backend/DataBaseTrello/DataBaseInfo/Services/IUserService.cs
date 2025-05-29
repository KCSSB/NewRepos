using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;

namespace DataBaseInfo.Services
{
    public interface IUserService
    {
        User? Register(string userName, string password);
        string Login(string userName, string password);
        //Task<User?> GetUserByIdAsync(int id);
        //Task<bool> CreateUserAsync(User user);
        //Task<bool> UpdateUserAsync(int id, User updatedUser);
        //Task<bool> DeleteUserAsync(int id);

    }
}
