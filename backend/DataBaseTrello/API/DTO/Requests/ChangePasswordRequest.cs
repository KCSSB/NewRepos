using API.Constants;
using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests
{
    public record ChangePasswordRequest
    {
        [Required(ErrorMessage = "Вы не указали старый")]
        [MinLength(AllowLengthProp.MinPassword, ErrorMessage = "Старый пароль должен содержать больше символов")]
        [MaxLength(AllowLengthProp.MaxPassword, ErrorMessage = "Старый должен содержать меньше символов")]
        public string OldPassword { get; init; }
        [Required(ErrorMessage = "Вы не указали новый пароль")]
        [MinLength(AllowLengthProp.MinPassword, ErrorMessage = "Новый Пароль должен содержать больше символов")]
        [MaxLength(AllowLengthProp.MaxPassword, ErrorMessage = "Новый Пароль должен содержать меньше символов")]
        public string NewPassword { get; init; }
    }
}
