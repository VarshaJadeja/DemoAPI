using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Demo.Repositories.Errors;

public class Errors 
{
    public class ErrorResponse
    {
        public string? ErrorType { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }

    public static int GetStatusCode(Error error)
    {
        if (error.Metadata.TryGetValue("StatusCode", out var statusCode))
        {
            return (int)statusCode;
        }

        return StatusCodes.Status500InternalServerError;
    }

    public static string GetErrorMessage(List<IReason> reasons)
    {
        return reasons.OfType<Error>().Select(e => e.Message).FirstOrDefault() ?? "An error occurred";
    }

    public static ErrorResponse CreateErrorResponse(List<IReason> reasons)
    {
        var firstError = reasons.OfType<Error>().FirstOrDefault() ?? new Error("Unknown error");

        return new ErrorResponse
        {
            ErrorType = firstError.Metadata.TryGetValue("ErrorType", out var errorType)
                        ? (string)errorType
                        : ErrorType.UnexpectedError.ToString(),
            Message = GetErrorMessage(reasons),
            StatusCode = GetStatusCode(firstError)

        };
    }

    public static IResult CreateResultFromErrors(List<IReason> reasons)
    {
        var errorResponse = CreateErrorResponse(reasons);
        return Results.Problem(
            detail: errorResponse.Message,
            statusCode: errorResponse.StatusCode
        );
    }
}

public enum ErrorType
{
    UserNotFound,
    InvalidCredentials,
    InvalidInput,
    UnexpectedError,
    UnAuthorized
}
