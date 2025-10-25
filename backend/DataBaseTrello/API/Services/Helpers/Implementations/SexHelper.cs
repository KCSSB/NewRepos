using DataBaseInfo.models;

namespace API.Services.Helpers.Implementations
{
    static class SexHelper
    {
        public static string GetSexDisplay(Sex sex) => sex switch
        {
            Sex.Male => "Мужской",
            Sex.Female => "Женский",
            _ => "Не указан"
        };
    }
}
