using Ambev.DeveloperEvaluation.Application.Common.Abstractions;

namespace Ambev.DeveloperEvaluation.Application.Common;

public class Result<T> : IResult
{
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
    public T? Value { get; set; }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(string error) =>
        new() { IsSuccess = false, Error = error };
    public static Result<T> Failure(string error, string message) =>
        new() { IsSuccess = false, Error = error, Message = message };
}
