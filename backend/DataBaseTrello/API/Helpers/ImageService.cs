using Imagekit.Sdk;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Options;
using API.Configuration;
namespace API.Helpers
{
    public class ImageService
        
    {
        private readonly ImagekitClient _imagekitClient;
        private readonly ImageKitSettings _settings;
        public ImageService(IOptions<ImageKitSettings> settings)
        {
            _settings = settings.Value;
            _imagekitClient = new ImagekitClient(publicKey: _settings.PublicKey,
                 privateKey: _settings.PrivateKey,
                 urlEndPoint: _settings.UrlEndpoint);
        }
        public async Task<IFormFile> PrepareImageAsync(IFormFile file, int height, int width)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            using var image = await Image.LoadAsync<Rgba32>(ms);

            if (image.Height != height || image.Width != width)
            {
                var bytes = await CropAndResizeAsync(image, height,width);
                var changedFile = ConvertBytesToFormFile(bytes, file.FileName, file.ContentType);
                return changedFile;
            }

            return file;

        }
        private async Task<byte[]> CropAndResizeAsync(Image<Rgba32> image, int height, int width)
        {
            using var outPutStream = new MemoryStream();
            int sizeOfSide = Math.Min(image.Width, image.Height);
            image.Mutate(x =>

            {
                x.Crop(new Rectangle((image.Width - sizeOfSide) / 2, (image.Height - sizeOfSide) / 2, height, width));
                x.Resize(new ResizeOptions
                {
                    Size = new Size(height, width),
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
        public async Task<Result?> UploadImageAsync(IFormFile file, string path)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(fileBytes);

            var request = new FileCreateRequest
            {
                file = $"data:{file.ContentType};base64,{base64}",
                fileName = file.FileName,
                useUniqueFileName = true,
                folder = $"{path}"
            };

            var result = await _imagekitClient.UploadAsync(request);

            return result;
        }
    }
}