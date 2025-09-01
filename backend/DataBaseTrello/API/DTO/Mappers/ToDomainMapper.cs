using API.Constants;
using API.DTO.Domain;
using API.DTO.Requests;
using API.Exceptions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackExchange.Redis;
using API.Exceptions.Context;

namespace API.DTO.Mappers
{
    public static class ToDomainMapper
    {
        public static UpdateUserModel ToUpdateUserModel(UpdateUserRequest request)
        {
            return new UpdateUserModel
            {
                FirstUserName = request.FirstUserName,
                LastUserName = request.LastUserName,
                Sex = request.Sex
            };
        }
        public static SessionData ToSessionData(HashEntry[] hashEntries)
        {
            var _errCreator = new ErrorContextCreator(typeof(ToDomainMapper).Name);
            var IsRevokedValue = hashEntries.FirstOrDefault(he => he.Name == "IsRevoked");
            if (IsRevokedValue.Value.IsNull)
                throw new AppException(_errCreator.Unauthorized("Неверные данные сессии"));

            return new SessionData()
            {
                IsRevoked = (bool)IsRevokedValue.Value,
            };
        }
    }
}
