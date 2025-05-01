using Concert_Ticket_Management_System.Domain.Entities;

namespace Concert_Ticket_Management_System.Shared;

public class Result<T>
{
    public T? Data { get; }
    public IEnumerable<string>? Errors { get; set; }
    public bool IsSuccess => Errors == null || !Errors.Any();

    private Result(T? data, IEnumerable<string>? errors)
    {
        Data = data;
        Errors = errors;
    }

    // Fix: Removed invalid constructor call and added a valid constructor
    private Result(bool isSuccess, T? data, IEnumerable<string>? errors)
    {
        Data = data;
        Errors = errors;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null);
    }

    public static Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>(false, default, errors);
    }
}
