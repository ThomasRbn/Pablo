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
        if (exception is not IProblemDetailsException problem)
        {
            return false;
        }

        httpContext.Response.StatusCode = problem.StatusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = problem.StatusCode,
                Title = problem.Title,
                Detail = problem.Detail,
            },
        });
    }
}