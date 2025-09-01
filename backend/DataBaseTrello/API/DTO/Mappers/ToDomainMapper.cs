using API.DTO.Domain;
using API.DTO.Requests;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StackExchange.Redis;

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
            var IsRevokedValue = hashEntries.FirstOrDefault(he => he.Name == "IsRevoked");
            if (IsRevokedValue.Value.IsNull)
                throw new AppException("Некорректные данные поле IsRevoked не найдено");

            return new SessionData()
            {
                IsRevoked = (bool)IsRevokedValue.Value,
            };
        }
    }
}
