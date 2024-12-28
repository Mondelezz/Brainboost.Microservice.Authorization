namespace API.Options;

public class JsonWebKeyOptions
{
    public required string Kid { get; set; }
    public required string Kty { get; set; }
    public required string Alg { get; set; }
    public required string Use { get; set; }
    public required string N { get; set; }
    public required string E { get; set; }
    public required string X5t { get; set; }
    public required string X5tS256 { get; set; }
}
