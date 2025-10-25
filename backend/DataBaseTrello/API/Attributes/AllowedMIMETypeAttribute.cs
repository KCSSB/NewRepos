using System.ComponentModel.DataAnnotations;

namespace API.Attributes
{
    public class AllowedMIMETypeAttribute : ValidationAttribute
    {
        private readonly string[] _MIMETypes;
        public AllowedMIMETypeAttribute(params string[] MIMETypes) => _MIMETypes = MIMETypes;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
               
                if (!_MIMETypes.Contains(file.ContentType.ToLowerInvariant()) )
                {
                    return new ValidationResult($"Ошибка: Неверный MIME Type файла: {file.ContentType}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
