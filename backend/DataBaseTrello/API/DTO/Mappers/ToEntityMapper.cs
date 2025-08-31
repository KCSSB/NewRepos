using API.DTO.Domain;
using DataBaseInfo.models;

namespace API.DTO.Mappers
{
    public static class ToEntityMapper
    {
        public static void ApplyToUser(UpdateUserModel model, User target)
        {
            if (!string.IsNullOrEmpty(model.FirstUserName))
                target.FirstName = model.FirstUserName;
            if (!string.IsNullOrEmpty(model.LastUserName))
                target.SecondName = model.LastUserName;
            if (model.Sex != null)
                target.Sex = (Sex)model.Sex;
        }
    }
}
