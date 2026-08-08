using Microsoft.AspNetCore.Diagnostics;

using Pablo.API.Exceptions;

namespace Pablo.API.Middleware;

public sealed class ProblemDetailsExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not IProblemDetailsException)
        {
            return false;
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
        });
    }
}