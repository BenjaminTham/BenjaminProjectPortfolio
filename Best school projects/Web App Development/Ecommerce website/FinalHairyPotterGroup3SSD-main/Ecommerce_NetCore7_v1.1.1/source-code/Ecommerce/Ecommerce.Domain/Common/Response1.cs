namespace Ecommerce.Domain.Common;
public class Response1<T>
{
    public bool Succeeded { get; protected set; }
    public T? Data { get; protected set; }
    public string? Message { get; protected set; }
    public List<string>? Errors { get; set; }

    public Response1() { }
    public Response1(T data, string message)
    {
        Succeeded = true;
        Data = data;
        Message = message;
    }

    public Response1(string message)
    {
        Succeeded = false;
        Message = message;
    }
    public static Response1<T> Success()
    {
        var result = new Response1<T> { Succeeded = true };
        return result;
    }
    public static Response1<T> Success(string message)
    {
        var result = new Response1<T> { Succeeded = true, Message = message };
        return result;
    }

    public static Response1<T> Success(T data, string message)
    {
        var result = new Response1<T> { Succeeded = true, Data = data, Message = message };
        return result;
    }

    public static Response1<T> Fail()
    {
        var result = new Response1<T> { Succeeded = false };
        return result;
    }

    public static Response1<T> Fail(string message)
    {
        var result = new Response1<T> { Succeeded = false, Message = message };
        return result;
    }

    public static Response1<T> Fail(T data, string message)
    {
        var result = new Response1<T> { Succeeded = false, Data = data, Message = message };
        return result;
    }

    public static Response1<T> Fail(string message, List<string> errors)
    {
        var result = new Response1<T> { Succeeded = false, Message = message, Errors = errors };
        return result;
    }

    public static Response1<T> Fail(List<string> errors)
    {
        var result = new Response1<T> { Succeeded = false, Errors = errors };
        return result;
    }

    public override string ToString()
    {
        return Succeeded ? Message : Errors == null || Errors.Count == 0 ? Message : $"{Message} : {string.Join(",", Errors)}";
    }
}

