using API.DTO.Domain;
using API.DTO.Requests;

namespace API.DTO.Mappers.ToDomainModel
{
    public static class ToDomainModelMapper
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
