using Keycloak.Auth.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using API.Options;
using Application;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Serilog;
using API.Extensions;

namespace API;

internal static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        AuthenticationOptions authOptions = builder.Configuration
        .GetSection(nameof(AuthenticationOptions))
        .Get<AuthenticationOptions>()!;

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

        builder.Host.UseSerilog();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.BackchannelHttpHandler = new BackChannelListener();

                o.BackchannelTimeout = TimeSpan.FromSeconds(30);

                o.RequireHttpsMetadata = true;
                o.Authority = authOptions.Authority;
                o.MetadataAddress = authOptions.MetadataAddress;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authOptions.ValidIssuer,
                    ValidAudiences = authOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                };
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    authOptions.MetadataAddress,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = false })
                {
                    RefreshInterval = TimeSpan.FromMinutes(60),
                    AutomaticRefreshInterval = TimeSpan.FromMinutes(60)
                };

                o.Events = new JwtBearerEvents
                {
                    // Логируем ошибку аутентификации
                    OnAuthenticationFailed = context =>
                    {
                        Log.Information("Authentication failed: " + context.Exception.Message);
                        Log.Information("Token: " + context.Request.Headers.Authorization);
                        return Task.CompletedTask;
                    },
                    // Логируем успешную валидацию токена
                    OnTokenValidated = context =>
                    {
                        Log.Information("Token validated: " + context.SecurityToken);
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddMvc();

        builder.Services.RegisterApplicationLayer(builder.Configuration);

        builder.Services.AddJaeger();
        builder.Services.AddPrometheus();

        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseCors(builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();

            builder.WithOrigins(
                "http://localhost:5000",
                "https://localhost:5001")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }

        app.UseRouting();

        app.UseOpenTelemetryPrometheusScrapingEndpoint();

        app.MapPrometheusScrapingEndpoint();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
