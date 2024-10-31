using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Application.Abstraction.UserAbstraction;
using Application.UserFeatures.UserService;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
