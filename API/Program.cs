using Keycloak.Auth.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using API.Options;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information($"Starting application: {typeof(Program).Assembly.GetName().Name}");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    AuthenticationOptions authOptions = builder.Configuration
        .GetSection(nameof(AuthenticationOptions))
        .Get<AuthenticationOptions>()!;

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGenWithAuth(builder.Configuration);

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.Audience = authOptions.Audience;
            o.MetadataAddress = authOptions.MetadataAddress;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = authOptions.ValidIssuer
            };
        });
    builder.Services.AddAuthorization();

    builder.Services.AddJaeger();
    builder.Services.AddPrometheus();

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseOpenTelemetryPrometheusScrapingEndpoint();

    app.MapPrometheusScrapingEndpoint();

    app.UseHttpsRedirection();

    app.MapGet("users/me", (ClaimsPrincipal claimsPrincipal) =>
    {
        return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
    }).RequireAuthorization();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
