using Application.Services.PhotoService;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class UserService(UserManager<AppUser> UserManager, IHttpContextAccessor HttpContext, ILocalPhotoService localPhotoService) : IUserService
{
    private ILocalPhotoService _localPhotoService { get; } = localPhotoService;
    private UserManager<AppUser> _userManager { get; } = UserManager;
    private IHttpContextAccessor _httpContext { get; } = HttpContext;

    public async Task<AppUserDto> GetUserDataAsync()
    {
        string username = _httpContext.HttpContext?.User.Identity?.Name!;
        AppUser user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == username) 
            ?? throw new Exception("User not found");
        return new(user.Id.ToString(), user.UserName!, user.Email!, user.PictureUrl);
    }

    public async Task UploadUserProfileAsync(IFormFile file)
    {
        string filename = await _localPhotoService.UploadPhoto(file, "profile-images");
        string username = _httpContext.HttpContext?.User.Identity?.Name!;
        AppUser user = _userManager.Users.FirstOrDefault(x => x.UserName == username) ?? throw new Exception("User not found");
        user.PictureUrl = filename;
        await _userManager.UpdateAsync(user);
    }

    public async Task<bool> UserExists(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? true : false;
    }
}

