using API;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.Keycloak;

namespace Keycloak.Tests;

public sealed class KeycloakFactoryFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public string? BaseAddress { get; set; } = "https://localhost:8443";

    private readonly KeycloakContainer _keycloak = new KeycloakBuilder()
        .WithImage("keycloak/keycloak:26.0")
        .WithPortBinding(8443, 8443)
        .WithName("keycloakTestContainer")
        .WithResourceMapping("./Certs", "/opt/keycloak/certs")
        .WithResourceMapping("./Import/Brainboost-realm.json", "/opt/keycloak/data/import")
        .WithEnvironment("KC_HTTPS_CERTIFICATE_FILE", "/opt/keycloak/certs/certificate.crt")
        .WithEnvironment("KC_HTTPS_CERTIFICATE_KEY_FILE", "/opt/keycloak/certs/certificate.key")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8443))
        .WithCleanUp(true)
        .Build();

    async Task IAsyncLifetime.InitializeAsync() => await _keycloak.StartAsync();

    async Task IAsyncLifetime.DisposeAsync() => await _keycloak.StopAsync();
}
