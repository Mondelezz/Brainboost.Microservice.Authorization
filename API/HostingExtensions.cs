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
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Host.UseSerilog();

        //TODO: В рабочей среде заменить на динамическое получение ключа подписи.

        #region *** Create jwk with appsettings.json ***
        JsonWebKeyOptions jwkOptions = builder.Configuration
            .GetSection(nameof(JsonWebKeyOptions))
            .Get<JsonWebKeyOptions>()!;
        #endregion

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
                    ValidateLifetime = true,
                    IssuerSigningKey = new JsonWebKey()
                    {
                        Kid = jwkOptions.Kid,
                        Kty = jwkOptions.Kty,
                        Alg = jwkOptions.Alg,
                        Use = jwkOptions.Use,
                        N = jwkOptions.N,
                        E = jwkOptions.E,
                        X5t = jwkOptions.X5t,
                        X5tS256 = jwkOptions.X5tS256
                    }
                };
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    authOptions.MetadataAddress,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = true })
                {
                    RefreshInterval = TimeSpan.FromMinutes(60),
                    AutomaticRefreshInterval = TimeSpan.FromMinutes(60)
                };

                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Information("Authentication failed: " + context.Exception.Message);
                        Log.Information("Token: " + context.Request.Headers.Authorization);
                        Log.Information("Exception details: " + context.Exception.ToString());
                        return Task.CompletedTask;
                    },
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
