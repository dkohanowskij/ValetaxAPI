namespace FxNet.Test.Api.Models;

public class Partner
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
