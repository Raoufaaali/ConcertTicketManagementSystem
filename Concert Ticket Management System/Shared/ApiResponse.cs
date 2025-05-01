namespace Concert_Ticket_Management_System.Shared;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public IEnumerable<string>? Messages { get; set; }
    public T? Data { get; set; }

    public ApiResponse(bool success, IEnumerable<string>? messages, T? data = default)
    {
        Success = success;
        Messages = messages;
        Data = data;
    }
}