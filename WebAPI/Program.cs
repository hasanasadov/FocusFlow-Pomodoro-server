using Application;
using Infrastructure;
using Infrastructure.Persistence.Context;
using Infrastructure.Services.Hubs;
using WebAPI.Filters;
using WebAPI.Handlers;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

builder.Services.AddScoped<AppDbContextInitializer>();

builder.Services.AddScoped<IAuthorizationHandler, UserExistsHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserExists", policy =>
        policy.Requirements.Add(new UserExistsRequirement()));
});

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<GroupRolePermissionFilter>();
});
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            string react_url = Environment.GetEnvironmentVariable("REACT_URL") ?? "http://localhost:3000";
            string react_urls = Environment.GetEnvironmentVariable("REACT_URL_S") ?? "https://localhost:3000";
            builder.WithOrigins(react_url, react_urls, "http://localhost:5501", "http://127.0.0.1:5501")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContextInitializer>();
    await context.InitialiseAsync();
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<RefreshTokenMiddleware>();
app.UseAuthorization();
app.UseStaticFiles();
app.MapHub<SignalHub>("/signal");
app.MapControllers();


app.Run();
