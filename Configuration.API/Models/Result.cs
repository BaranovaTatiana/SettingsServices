namespace Configuration.API;

public class Result(Status status, string message)
{
    public Status Status { get; set; } = status;
    public string Message { get; set; } = message;
}