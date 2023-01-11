using System;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuberDinner.Infrastructure;

public static class DependencyInjectionRegister
{
	public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
	{
        
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SECTION_NAME));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        
        return services;
    }
}


