using System.IdentityModel.Tokens.Jwt;
using System.Net;
using API.Exceptions.ErrorContext;
using Microsoft.IdentityModel.Tokens;
using API.Constants;

namespace API.Helpers
{
    public class TokenExtractorService
    {
        public  int TokenExtractorId(string accessToken)
        {
        
        var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(accessToken))
                    throw new AppException(new ErrorContext(ServiceName.TokenExtractorService,
                     OperationName.TokenExtractorId,
                    HttpStatusCode.InternalServerError,
                    "Произошла внутренняя ошибка в момент выполнения операции",
                     $"Ошибка,неверный формат accessToken: {accessToken}"));

                var token = jwtHandler.ReadJwtToken(accessToken);
        var UserIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == "UserId");
            if (UserIdClaim == null)
                throw new AppException(new ErrorContext(ServiceName.TokenExtractorService,
                     OperationName.TokenExtractorId,
                    HttpStatusCode.InternalServerError,
                    "Произошла внутренняя ошибка в момент выполнения операции",
                     $"Ошибка, не удалось получить UserId из токена, Claim в access token, не содержит UserId"));
            int UserId = int.Parse(UserIdClaim.Value);
            return UserId;
        }
    }
}
