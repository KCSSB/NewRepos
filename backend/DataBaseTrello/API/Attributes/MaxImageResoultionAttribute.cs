using System.ComponentModel.DataAnnotations;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
namespace API.Attributes
{
    public class MaxImageResoultionAttribute :ValidationAttribute
    {
            private readonly int _maxHeight;
            private readonly int _maxWidth;
            public MaxImageResoultionAttribute(int maxWidth, int maxHeight)
            {
                _maxHeight = maxHeight;
                _maxWidth = maxWidth;
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
                    if (_maxHeight < image.Height || _maxWidth < image.Width)
                    {
                        return new ValidationResult($"Разрешение изображение больше установленного максимального изображения: {_maxWidth}x{_maxHeight}");
                    }
                }
                return ValidationResult.Success;
            }
        }
    }



