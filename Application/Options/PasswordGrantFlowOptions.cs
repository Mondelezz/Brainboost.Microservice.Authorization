namespace Application.Options;

internal class PasswordGrantFlowOptions
{
    public string KeycloakUrl { get; set; } = string.Empty;
    public string Realm { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
