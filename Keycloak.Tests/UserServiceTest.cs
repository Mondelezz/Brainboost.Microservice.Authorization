using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Keycloak.Tests;

[Collection(nameof(KeycloakFactoryFixtureCollection))]
public sealed class UserServiceTest(KeycloakFactoryFixture keycloakFactory)
{
    private readonly HttpClient _httpClient = keycloakFactory.CreateClient();
    private readonly string _baseAddress = keycloakFactory.BaseAddress
        ?? string.Empty;
    private readonly HttpClient _client = new();

    /// <summary>
    /// Область и клиент сервера Keycloak
    /// </summary>
    private const string Realm = "Brainboost";
    private const string Client = "brainboost";

    /// <summary>
    /// Данные для тестирования
    /// </summary>
    private const string Username = "Vlad";
    private const string Password = "1q2w3e4r";
    private const string GrantType = "password";

    [Fact]
    public async Task GetMyProfile_ShouldReturnOk()
    {
        // Arrange
        string urlToken = $"{_baseAddress}/realms/{Realm}/protocol/openid-connect/token";
        const string requestUri = "api/v1/users/get-my-profile";

        Dictionary<string, string> data = new()
        {
            { "grant_type", GrantType },
            { "client_id", Client },
            { "username", Username },
            { "password", Password }
        };

        HttpResponseMessage response = await _client.PostAsync(urlToken, new FormUrlEncodedContent(data));
        JsonObject? content = await response.Content.ReadFromJsonAsync<JsonObject>();
        string? token = content?["access_token"]?.ToString();

        // Act
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage result = await _httpClient.GetAsync(requestUri);

        //Assert
        Assert.True(result.IsSuccessStatusCode, "Expected success status code (200), but got: " + result.StatusCode);
    }
}
