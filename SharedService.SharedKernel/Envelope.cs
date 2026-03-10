using System.Text.Json.Serialization;

namespace SharedService.SharedKernel;

public record Envelope
{
    public object? Result { get; }
    public Failure? Errors { get; }
    public DateTime TimeGenerate { get; }

    [JsonConstructor]
    private Envelope(object? result, Failure? errors,  DateTime timeGenerate)
    {
        Result = result;
        Errors = errors;
        TimeGenerate = timeGenerate;
    }

    public static Envelope Ok(object? result = null) => new(result, null,  DateTime.UtcNow);
    public static Envelope Error(Failure errors) => new(null, errors, DateTime.UtcNow);
}

public record Envelope<T>
{
    public T? Result { get; }
    public Failure? Errors { get; }
    public DateTime TimeGenerate { get; }

    [JsonConstructor]
    private Envelope(T? result, Failure? errors,  DateTime timeGenerate)
    {
        Result = result;
        Errors = errors;
        TimeGenerate = timeGenerate;
    }

    public static Envelope<T> Ok(T? result = default) => new(result, null, DateTime.UtcNow);
    public static Envelope<T> Error(Failure errors) => new(default, errors, DateTime.UtcNow);
}
