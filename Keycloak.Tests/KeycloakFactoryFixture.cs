using API;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.Keycloak;

namespace Keycloak.Tests;

public sealed class KeycloakFactoryFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public string? BaseAddress { get; set; } = "https://localhost:4444";

    private readonly KeycloakContainer _keycloak = new KeycloakBuilder()
    .WithImage("quay.io/keycloak/keycloak:26.0.7")
    .WithName("keycloak")
    .WithPortBinding(4444, 8080)
    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
    .WithCleanUp(true)
    .Build();

    async Task IAsyncLifetime.InitializeAsync() => await _keycloak.StartAsync();

    async Task IAsyncLifetime.DisposeAsync() => await _keycloak.StopAsync();
}
