namespace FxNet.Test.Api.Models;

public class ExceptionJournal
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Parameters { get; set; }
    public string? StackTrace { get; set; }
    public string? Text { get; set; }
}
