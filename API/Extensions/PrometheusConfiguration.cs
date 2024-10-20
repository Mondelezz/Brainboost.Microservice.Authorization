using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Keycloak.Auth.Api.Extensions;

public static class PrometheusConfiguration
{
    public static IServiceCollection AddPrometheus(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(opts => opts
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Keycloak.Auth.Api"))
            .AddPrometheusExporter()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel")
            .AddView("request-duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = [0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5, 10]
            }));

        return services;
    }
}
