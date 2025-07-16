using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers
{
    public class TokenExtractorService
    {
        public  int TokenExtractorId(string? accessToken)
        {
            try
            {
                if (string.IsNullOrEmpty(accessToken)) ;
                throw new Exception("Ошибка при получении accessToken");
        var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(accessToken))
                throw new SecurityTokenException("Неверный формат, невозможно прочитать JWT");

        var token = jwtHandler.ReadJwtToken(accessToken);
        var UserIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == "UserId");
            if (UserIdClaim == null)
                throw new FormatException("Claim UserId is missing");
        int UserId = int.Parse(UserIdClaim.Value);
            return UserId;
            }
            catch (SecurityTokenException)
            {
                //Неверный формат JWT
                throw;
            }
            catch (Exception)
            {
                //Клеймы отсутствуют
                throw;
            }
            catch(Exception)
            {
                //Ошибка при получении AccessToken
                throw;
            }
        }
    }
}
