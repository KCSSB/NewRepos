using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers
{
    public class TokenExtractorService
    {
        public  int TokenExtractorId(string accessToken)
        {
        var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(accessToken))
                throw new SecurityTokenException("Invalid token format: cannot read JWT");

        var token = jwtHandler.ReadJwtToken(accessToken);
        var UserIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == "UserId");
            if (UserIdClaim == null)
                throw new FormatException("Claim UserId is missing");
        int UserId = int.Parse(UserIdClaim.Value);
            return UserId;
        }
    }
}
