using API.Options;
using Microsoft.OpenApi.Models;

namespace Keycloak.Auth.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        KeycloakOptions keycloakOptions = configuration
            .GetSection(nameof(KeycloakOptions))
            .Get<KeycloakOptions>()!;

        services.AddSwaggerGen(o =>
        {
            #region SwaggerDoc
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Brainboost.Authorization",
                Version = "v1",
                Description = "Microservice module API with Keycloak. This template based on .NET 8.0\n",
                Contact = new OpenApiContact
                {
                    Url = new Uri("https://github.com/Mondelezz"),
                    Email = @"pankov.egor26032005@yandex.ru",
                    Name = "Mondelezz"
                },
            });
            #endregion

            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(keycloakOptions.AuthorizationUrl),
                        TokenUrl = new Uri(keycloakOptions.TokenUrl),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" }
                        }
                    }
                }
            });

            OpenApiSecurityRequirement securityRequirement = new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = "Bearer",
                        Scheme = "Bearer",
                    },
                    new List<string> { "openid", "profile" }
                }
            };

            o.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }
}

