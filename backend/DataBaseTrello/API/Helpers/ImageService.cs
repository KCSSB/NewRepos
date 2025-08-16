using API.Constants;
using API.Exceptions.ErrorContext;
using Imagekit.Sdk;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
namespace API.Helpers
{
    public class ImageService
        
    {
        private readonly ImagekitClient _imagekitClient;
        public ImageService(ImagekitClient imagekitClient)
        {
            _imagekitClient = imagekitClient;
        }
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
                    Size = new Size(size, size),
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
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(fileBytes);

            var request = new FileCreateRequest
            {
                file = $"data:{file.ContentType};base64,{base64}",
                fileName = file.FileName,
                useUniqueFileName = true,
                folder = "/UsersAvatars"
            };
            var result = await _imagekitClient.UploadAsync(request);
            if (result.HttpStatusCode >= 200 && result.HttpStatusCode < 300)
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new AppException(new ErrorContext(ServiceName.UserService,
                 OperationName.UploadUserAvatarAsync,
                 HttpStatusCode.InternalServerError,
                UserExceptionMessages.UploadFilesExceptionMessage,
                $"Произошла ошибка в момент смены аватара пользователя, Пользователь id: {userId}, не найден"));


                user.Avatar = result.url;
                await context.SaveChangesWithContextAsync(ServiceName.UserService,
                    OperationName.UploadUserAvatarAsync,
                    $"Произошла ошибка в момент смены аватара пользователя, не удалось сохранить url: {result.url}",
                    UserExceptionMessages.UploadFilesExceptionMessage,
                    HttpStatusCode.InternalServerError);

                return result.url;
            }
            else
            {
                throw new AppException(new ErrorContext(
            ServiceName.ImageService,
            OperationName.UploadImageAsync,
            (HttpStatusCode)result.HttpStatusCode,
            UserExceptionMessages.UploadFilesExceptionMessage,
            $"Ошибка при загрузке изображения в ImageKit. Код: {result.HttpStatusCode}"));
            }
        }
    }
}