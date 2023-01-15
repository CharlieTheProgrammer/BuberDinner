using System;
using System.Text;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Infrastructure.Authentication;
using BuberDinner.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuberDinner.Infrastructure;

public static class DependencyInjectionRegisters
{
	public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
	{
		// Instantiating the class here, which means we can't use JwtSettings.SECTION_NAME in the Configure
		var jwtSettings = new JwtSettings();
		
		// If we don't Bind, .NET doesn't know which values to bind, so the object will have the properties,
		// but they will be null. Bind tells .NET from where to populate the values
		configuration.Bind(JwtSettings.SECTION_NAME, jwtSettings);
		
		// Other parts of the code still rely on IOptions. This will add a singleton so the changes above doesn't
		// break the existing code.
		services.AddSingleton(Options.Create(jwtSettings));
		
		// This no longer works if we want to access JwtSettings directly here.
		// services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SECTION_NAME));

		services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IUserRepository, UserRepository>();
        
        // Add Authentication here.
        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
	        .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters()
		        {
			        ValidateIssuer = true,
			        ValidateAudience = true,
			        ValidateLifetime = true,
			        ValidateIssuerSigningKey = true,
			        ValidIssuer = jwtSettings.Issuer,
			        ValidAudience = jwtSettings.Audience,
			        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
		        });
        
        return services;
    }
}


