using Keycloak.Auth.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using API.Options;
using Application;

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

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;

                o.Authority = authOptions.Authority;
                o.Audience = authOptions.Audience;
                o.MetadataAddress = authOptions.MetadataAddress;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authOptions.ValidIssuer
                };
            });

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
                "http://localhost:4200",
                "https://localhost:4200")
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
