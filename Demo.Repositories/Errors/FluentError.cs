using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Demo.Repositories.Errors;

public class FluentError
{
    private static readonly Dictionary<ErrorType, int> ErrorStatusCodes = new()
    {
        { ErrorType.UserNotFound, StatusCodes.Status404NotFound },
        { ErrorType.InvalidCredentials, StatusCodes.Status400BadRequest },
        { ErrorType.InvalidInput, StatusCodes.Status400BadRequest },
        { ErrorType.UnexpectedError, StatusCodes.Status500InternalServerError },
        { ErrorType.UnAuthorized, StatusCodes.Status401Unauthorized }
    };

    public static Error NotFound(ErrorType errorType, string message)
    {
        return new Error(message)
            .WithMetadata("ErrorType", errorType.ToString())
            .WithMetadata("StatusCode", ErrorStatusCodes[errorType]);
    }

    public static Error InvalidCredentials(ErrorType errorType, string message)
    {
        return new Error(message)
            .WithMetadata("ErrorType", errorType.ToString())
            .WithMetadata("StatusCode", ErrorStatusCodes[errorType]);
    }
    public static Error UnAuthorized(ErrorType errorType, string message)
    {
        return new Error(message)
            .WithMetadata("ErrorType", errorType.ToString())
            .WithMetadata("StatusCode", ErrorStatusCodes[errorType]);
    }
}
