using Microsoft.CodeAnalysis.CSharp.Syntax;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace API.Helpers
{
    public class ImageService
    {
        public async Task<IFormFile> PrepareImageAsync(IFormFile file, int size)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            using var image = await Image.LoadAsync<Rgba32>(ms);
            if (image.Height != size || image.Width != size)
            {

                var bytes = await CropAndResizeAsync(image, size);
                var changedFile = ConvertBytesToFormFile(bytes, file.FileName, file.ContentType);
                return changedFile;
            }

            return file;

        }
        private async Task<byte[]> CropAndResizeAsync(Image<Rgba32> image, int size)
        {
            using var outPutStream = new MemoryStream();
            int sizeOfSide = Math.Min(image.Width, image.Height);
            image.Mutate(x =>

            {
                x.Crop(new Rectangle((image.Width - sizeOfSide) / 2, (image.Height - sizeOfSide) / 2, size, size));
                x.Resize(new ResizeOptions
                {
                    Size = new Size(1024, 1024),
                    Mode = ResizeMode.Crop,

                });
            });



            await image.SaveAsJpegAsync(outPutStream);
            return outPutStream.ToArray();
        }
        public static IFormFile ConvertBytesToFormFile(byte[] fileBytes, string fileName, string contentType)
        {
            var stream = new MemoryStream(fileBytes);

            var formFile = new FormFile(stream, 0, fileBytes.Length, name: "file", fileName: fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }
    }
}