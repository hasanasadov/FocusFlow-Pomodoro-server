namespace Application.DTOs;

public sealed record AppUserDto(string Id, string Username, string Email, string ImagePath);

public sealed record LoginDto(string EmailOrUsername, string Password);

public sealed record RegisterDto(string Username, string Email, string Password);