namespace Ambev.DeveloperEvaluation.Application.Common.Abstractions;

public interface IResult
{
    bool IsSuccess { get; }
    string? Message { get; }
    string? Error { get; }
}
