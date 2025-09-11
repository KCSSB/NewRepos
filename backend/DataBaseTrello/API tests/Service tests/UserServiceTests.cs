using API.DTO.Domain;

namespace API_tests
{
    public class UserServiceTests
    {
        [Fact]
 
        public Task<int> RegisterAsync(string userEmail, string password)
        {

        }
        public Task<(string AccessToken, string RefreshToken)> LoginAsync(string userEmail, string password, string? deviceId)
        {

        }
        public Task<string> UpdateUserAvatarAsync(Result? result, int userId)
        {

        }
        public Task<UpdateUserModel> UpdateUserAsync(UpdateUserModel model, int userId)
        {

        }
        public Task ChangePasswordAsync(string oldPass, string newPass, int userId)
        {

        }
    }

}