namespace Application.Options;

internal class PasswordGrantFlowOptions
{
    public required string KeycloakUrl { get; set; }
    public string Realm { get; set; } = string.Empty;
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
