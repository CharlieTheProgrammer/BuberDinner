using System;
using Microsoft.Extensions.DependencyInjection;

namespace BuberDinner.Infrastructure;

public static class DependencyInjectionRegister
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
        // services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}


