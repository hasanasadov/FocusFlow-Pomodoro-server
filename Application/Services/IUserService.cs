using Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public interface IUserService
{
    Task<AppUserDto> GetUserDataAsync();
    Task UploadUserProfileAsync(IFormFile file);
    Task<bool> UserExists(string userId);
}

