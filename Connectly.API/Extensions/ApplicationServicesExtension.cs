
using Connectly.Service.Photos;

namespace Connectly.API.Extensions;


public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<LogUserActivity>();
        services.AddScoped<IPhotoService, PhotoService>();

        services.AddAutoMapper(typeof(MappingProfiles));

        return services;
    }
}

