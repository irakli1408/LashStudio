namespace LashStudio.Infrastructure.Logs;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string Level { get; set; } = "Error";
    public string Message { get; set; } = "";
    public string? StackTrace { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public string? TraceId { get; set; }
}
