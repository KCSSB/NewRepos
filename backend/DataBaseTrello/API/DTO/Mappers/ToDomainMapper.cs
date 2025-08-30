using API.DTO.Domain;
using API.DTO.Requests;

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
    }
}
