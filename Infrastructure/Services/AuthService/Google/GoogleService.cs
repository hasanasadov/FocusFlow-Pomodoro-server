using Application.Services.AuthService;
using Application.Services.TokenService;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services.AuthService.Google;

public class GoogleService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleService(UserManager<AppUser> userManager, IConfiguration configuration, ITokenService tokenService, IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
        _httpClientFactory = httpClientFactory;
    }

    public class UserInfoDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("picture")]
        public string Picture { get; set; }
        [JsonPropertyName("sub")]
        public string Sub { get; set; }
    }

    public async Task AuthenticateAsync(GoogleAuthenticateDto request)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.IdToken);
        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<UserInfoDto>(responseData);

        var user = 
            await _userManager.FindByLoginAsync("GOOGLE", payload.Sub) ?? 
            await _userManager.FindByEmailAsync(payload.Email);

        if (user is null)
        {
            user = new AppUser
            {
                Email = payload.Email,
                UserName = payload.GivenName,
                PictureUrl = payload.Picture
            };

            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user, new("GOOGLE", payload.Sub, "GOOGLE"));
        }
        await _tokenService.GenerateToken(user, await _userManager.GetRolesAsync(user), CancellationToken.None);
    }
}

