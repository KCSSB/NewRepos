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
        (string AcessToken, string? RefreshToken) Login(string UserEmail, string Password);
        

    }
}
