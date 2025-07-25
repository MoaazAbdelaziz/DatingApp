using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"))
        );

        services.AddCors();

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IMemberRepository, MemberRepository>();

        services.AddScoped<IPhotoService, PhotoService>();

        services.AddScoped<ILikesRepository, LikesRepository>();

        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddScoped<LogUserActivity>();

        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

        services.AddSignalR();

        services.AddSingleton<PresenceTracker>();

        return services;
    }
}
