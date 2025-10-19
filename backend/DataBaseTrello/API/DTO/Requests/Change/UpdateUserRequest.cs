using System.ComponentModel.DataAnnotations;
using API.Constants;
using DataBaseInfo.models;

namespace API.DTO.Requests.Change
{
    public record class UpdateUserRequest
    {
        [MaxLength(AllowLengthProp.MaxFirstUserName, ErrorMessage = "Имя пользователя должно быть длиннее")]
        [MinLength(AllowLengthProp.MinFirstUserName, ErrorMessage = "Имя пользователя должно быть короче")]
        [RegularExpression(@"^[\p{L}\-']+$", ErrorMessage = "Имя должно содержать только буквы, дефис или апостроф, без пробелов.")]
        public string? FirstUserName { get; set; }
        [MaxLength(AllowLengthProp.MaxLasttUserName, ErrorMessage = "Фамилия пользователя должно быть длиннее")]
        [MinLength(AllowLengthProp.MinLastUserName, ErrorMessage = "Фамилия пользователя должно быть короче")]
        [RegularExpression(@"^[\p{L}\-']+$", ErrorMessage = "Имя должно содержать только буквы, дефис или апостроф, без пробелов.")]
        public string? LastUserName { get; set; }
        [Range(0,2, ErrorMessage = "Значение пола должно являтся цифрой от 0 до 2")]
        public Sex? Sex { get; set; }
    }
}
