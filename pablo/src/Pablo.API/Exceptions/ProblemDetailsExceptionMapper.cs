using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Pablo.API.Exceptions;

public static class ProblemDetailsExceptionMapper
{
    public static void Apply(ProblemDetailsContext context, IProblemDetailsException problem)
    {
        context.HttpContext.Response.StatusCode = problem.StatusCode;
        context.ProblemDetails.Status = problem.StatusCode;
        context.ProblemDetails.Title = problem.Title;
        context.ProblemDetails.Detail = problem.Detail;

        if (problem.Errors is { Count: > 0 } errors)
        {
            context.ProblemDetails.Extensions["errors"] = errors;
        }
    }
}