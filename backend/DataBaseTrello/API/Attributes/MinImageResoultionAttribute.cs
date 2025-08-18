using System.ComponentModel.DataAnnotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace API.Attributes
{
    public class MinImageResoultionAttribute: ValidationAttribute
    {
        private readonly int _minHeight;
        private readonly int _minWidth;
        public MinImageResoultionAttribute(int minHeight, int minWidth)
        {
            _minHeight = minHeight;
            _minWidth = minWidth;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                using var ms = new MemoryStream();
                file.CopyTo(ms);
                ms.Position = 0;
                var image = Image.Identify(ms);
                if(_minHeight >  image.Height || _minWidth > image.Width)
                {
                    return new ValidationResult($"Разрешение изображение меньше установленного минимального изображения: {_minHeight}x{_minWidth}");
                } 
            }
            return ValidationResult.Success;
        }
    }
}
