using System.Net;
using System.Security.Claims;
using API.Constants;
using API.Exceptions;
using API.Exceptions.Context;


namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
   
        
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var _errCreator = new ErrorContextCreator(ServiceName.ClaimsPrincipalExtensions);
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdString == null)
                throw new AppException(_errCreator.NotFound("Ошибка при получении Claim NameIdentifier из User"));

            if (!int.TryParse(userIdString, out int userId))
                throw new AppException(_errCreator.BadRequest($"Ошибка при приведении userId {userIdString} к формату int"));

            return userId;
        }
    }
}
