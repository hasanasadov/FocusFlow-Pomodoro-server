using Application.Services.TokenService;

namespace Infrastructure.Services.TokenService
{
    public sealed class TokenService(
        IHttpContextAccessor contextAccessor,
        UserManager<AppUser> userManager,
        IOptions<TokenSetting> tokenSettings,
        AppDbContext appDbContext) : ITokenService
    {
        private readonly HttpContext _context = contextAccessor.HttpContext!;
        private readonly UserManager<AppUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly AppDbContext _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        private readonly TokenSetting _tokenSettings = tokenSettings?.Value ?? throw new ArgumentNullException(nameof(tokenSettings));


        public async Task<Result<string>> GenerateToken(AppUser user, IList<string> roles, CancellationToken cancellationToken)
        {
            if (user == null) return Result<string>.Failure(Error.BadRequest(UserTaskErrors.UserTaskNotFound));
            if (roles == null) return Result<string>.Failure(Error.BadRequest(TokenErrors.InvalidRole));

            var claims = GetClaims(user, roles);
            var token = CreateJwtToken(claims);
            var tokenHandler = new JwtSecurityTokenHandler();

            //var addClaimsResult = await _userManager.AddClaimsAsync(user, claims);
            //if (!addClaimsResult.Succeeded)
            //{
            //    return Result<string>.Failure(Error.InvalidRequest(TokenErrors.FailedToAddClaim));
            //}
            string accessToken = tokenHandler.WriteToken(token);
            AppendCookie(accessToken);
            await GenerateRefreshToken(user, cancellationToken);
            return Result<string>.Success(accessToken);
        }

        public async Task<Result<string>> GenerateRefreshToken(AppUser user, CancellationToken cancellationToken)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            
            string uniqueData = $"{user.Id}-{user.Email}-{DateTime.UtcNow.Ticks}";
            byte[] uniqueBytes = Encoding.UTF8.GetBytes(uniqueData);
            
            byte[] combinedBytes = randomNumber.Concat(uniqueBytes).ToArray();
            
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            
            string refreshToken = Convert.ToBase64String(hashBytes);
            
            user.RefreshToken = refreshToken;
            await _appDbContext.SaveChangesAsync(cancellationToken);
            AppendCookie(refreshToken, true);
            return Result<string>.Success(refreshToken);
        }

        public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret)),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Result<ClaimsPrincipal>.Failure(Error.InvalidRequest(TokenErrors.InvalidToken));
                }

                return Result<ClaimsPrincipal>.Success(principal);
            }
            catch (Exception)
            {
                return Result<ClaimsPrincipal>.Failure(Error.InvalidRequest(TokenErrors.InvalidToken));
            }
        }

        public async Task<Result<string>> GenerateTokenFromRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            var user = await ValidateRefreshToken(refreshToken);
            if (user == null)
            {
                return Result<string>.Failure(Error.InvalidRequest(TokenErrors.InvalidRefreshToken));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await GenerateToken(user, roles, cancellationToken);
            if (result.IsFailure)
            {
                return Result<string>.Failure(result.Error);
            }
            return Result<string>.Success(result.Value);
        }

        private List<Claim> GetClaims(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claims;
        }

        private JwtSecurityToken CreateJwtToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret));
            return new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                expires: DateTime.Now.AddDays(_tokenSettings.Expiration),
                claims: claims,
                
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
        }

        private void AppendCookie(string token, bool isRefresh = false)
        {
            if (!isRefresh)
            {
                CookieOptions option = new()
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMinutes(_tokenSettings.Expiration),
                    SameSite = SameSiteMode.Strict,
                    Secure = true
                };
                _context?.Response.Cookies.Append(_tokenSettings.CookieName, token, option);
                return;
            }

            CookieOptions refreshTokenOption = new()
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(_tokenSettings.ExpirationRefresh),
                SameSite = SameSiteMode.Strict,
                Secure = true
            };
            _context?.Response.Cookies.Append(_tokenSettings.RefreshCookieName, token, refreshTokenOption);
        }

        private async Task<AppUser?> ValidateRefreshToken(string refreshToken)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}
