namespace FxNet.Test.Api.DTOs;

public class MJournalInfo
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Text { get; set; }
}
