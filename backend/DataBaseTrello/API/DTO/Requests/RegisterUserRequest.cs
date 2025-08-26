using System.ComponentModel.DataAnnotations;
using API.Constants;
using Microsoft.AspNetCore.Mvc;

namespace API.DTO.Requests
{
    public record RegisterUserRequest
    {

        [Required(ErrorMessage = "Вы не указали email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [MinLength(AllowLengthProp.MinEmail, ErrorMessage = $"email должен содержать больше символов")]
        [MaxLength(AllowLengthProp.MaxEmail, ErrorMessage = "email должен содержать меньше символов")]
        public required string UserEmail { get; init; }

        [Required(ErrorMessage = "Вы не указали пароль")]
        [MinLength(AllowLengthProp.MinPassword, ErrorMessage = "Пароль должен содержать больше символов")]
        [MaxLength(AllowLengthProp.MaxPassword, ErrorMessage = "Пароль должен содержать меньше символов")]
        public required string UserPassword { get; init; }
    }
}
