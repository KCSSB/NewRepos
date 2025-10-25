namespace API.Services.Helpers.Interfaces
{
    public interface IImageService
    {
        Task<Result?> UploadImageAsync(IFormFile file, string path);
    }
}