using Application.Services.AuthService;
using Application.Services.AuthService.Local;
using Application.Services.CacheService;
using Application.Services.PhotoService;
using Application.Services.TokenService;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Services;
using Infrastructure.Services.AuthService.Google;
using Infrastructure.Services.AuthService.Local;
using Infrastructure.Services.CacheService;
using Infrastructure.Services.HostedServices;
using Infrastructure.Services.PhotoService;
using Infrastructure.Services.TokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddDataProtection();

        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequiredLength = 1;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireDigit = false;
            opt.Password.RequiredUniqueChars = 0;
            opt.User.RequireUniqueEmail = true;
        })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

        services.Configure<TokenSetting>(opt =>
        {
            opt.Issuer = configuration["TokenSettings:Issuer"]!;
            opt.Audience = configuration["TokenSettings:Audience"]!;
            opt.Secret = configuration["TokenSettings:Secret"]!;
            opt.Expiration = int.Parse(configuration["TokenSettings:Expiration"]!);
            opt.CookieName = configuration["TokenSettings:CookieName"]!;
            opt.ExpirationRefresh= int.Parse(configuration["TokenSettings:ExpirationRefresh"]!);
            opt.RefreshCookieName = configuration["TokenSettings:RefreshCookieName"]!;
        });

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
            opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenSettings:Secret"]!)),
                    ValidateLifetime = true,
                    ValidIssuer = configuration["TokenSettings:Issuer"],
                    ValidAudience = configuration["TokenSettings:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = ClaimTypes.Name
                };
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrEmpty(context.Token))
                            context.Token = context.Request.Cookies[configuration["TokenSettings:CookieName"]!];
                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddSignalR();
        services.AddHttpContextAccessor();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserTaskService, UserTaskService>();
        services.AddScoped<ITaskStepService, TaskStepService>();
        services.AddScoped<ILocalAuthService, LocalAuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILocalPhotoService, LocalPhotoService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IAuthService, GoogleService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IGroupRoleService, GroupRoleService>();
        services.AddScoped<IProjectService, ProjectService>();


        services.AddSingleton<ICacheService, RedisMultiplexerService>();

        services.AddHostedService<GroupMembershipInitializer>();


        return services;
    }
}
