using System.Net;
using System.Security.Claims;
using API.Constants;
using API.Exceptions.Context;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdString == null)
                throw new AppException(new ErrorContext(ServiceName.UserController,
                    OperationName.GetUserId,
                    HttpStatusCode.Unauthorized,
                    UserExceptionMessages.AuthorizeExceptionMessage,
                    "Ошибка при получении Claim NameIdentifier из User"));

            if (!int.TryParse(userIdString, out int userId))
                throw new AppException(new ErrorContext(ServiceName.UserController,
                    OperationName.GetUserId,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"Ошибка при приведении userId {userIdString} к формату int"));

            return userId;
        }
    }
}
