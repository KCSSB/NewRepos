using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace API.Attributes
{
    public class MaxFileSizeAttribute: ValidationAttribute
    {
        private readonly long _maxFileSize;
        public MaxFileSizeAttribute(long maxFileSize) => _maxFileSize = maxFileSize;
 
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           var file = value as IFormFile;
            if(file != null)
            {
                long fileSize = file.Length;
                if(fileSize > _maxFileSize)
                {
                    var maxSizeInMb = _maxFileSize / 1024 / 1024;
                    return new ValidationResult($"Ошибка: размер файла превышает допустимые {maxSizeInMb} МБ.");
                   
                }
            }
            return ValidationResult.Success;
        }
    }
}
