using ErrorOr;

namespace Chatty.Core.Api;

public static class Extensions
{
    public static IResult  HandleErrors(this IEnumerable<Error> errors)
    {
        var errorList = errors.ToList();
        var firstError = errorList.First();

        return firstError.Type switch
        {
            // Handle 401 Unauthorized
            ErrorType.Unauthorized => Results.Unauthorized(),

            // Handle 400 Bad Request
            ErrorType.Validation => Results.BadRequest(errorList),

            // Handle 404 Not Found
            ErrorType.NotFound => Results.NotFound(),

            // Fallback to 500 Internal Server Error or a custom error
            _ => Results.Problem("An unexpected error occurred.")
        };
    }
}
