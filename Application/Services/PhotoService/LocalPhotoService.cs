using Microsoft.AspNetCore.Http;

namespace Application.Services.PhotoService;

public interface ILocalPhotoService
{
    ValueTask<string> UploadPhoto(IFormFile file, string path);
}