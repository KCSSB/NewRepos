using System.Net;
using System.Security.Claims;
using API.Constants;
using API.Exceptions.ErrorContext;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdString == null)
                throw new AppException(new ErrorContext(ServiceName.UserController,
                    OperationName.GetUserIdAsGuidOrThrow,
                    HttpStatusCode.Unauthorized,
                    UserExceptionMessages.AuthorizeExceptionMessage,
                    "Ошибка при получении Claim NameIdentifier из User"));

            if (!Guid.TryParse(userIdString, out Guid userId))
                throw new AppException(new ErrorContext(ServiceName.UserController,
                    OperationName.GetUserIdAsGuidOrThrow,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"Ошибка при приведении userId {userIdString} к формату Guid"));

            return userId;
        }
    }
}
