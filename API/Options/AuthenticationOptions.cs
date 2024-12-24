namespace API.Options;

public class AuthenticationOptions
{
    public string MetadataAddress { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public IList<string> Audience { get; set; } = [];
    public string Authority { get; set; } = string.Empty;
}
