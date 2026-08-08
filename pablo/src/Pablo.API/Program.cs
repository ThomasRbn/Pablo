using Pablo.API.Exceptions;
using Pablo.API.Infrastructure;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            if (context.Exception is IProblemDetailsException problem)
            {
                context.ProblemDetails.Status = problem.StatusCode;
                context.ProblemDetails.Title = problem.Title;
                context.ProblemDetails.Detail = problem.Detail;
                context.HttpContext.Response.StatusCode = problem.StatusCode;

                if (problem.Errors is { Count: > 0 } errors)
                {
                    context.ProblemDetails.Extensions["errors"] = errors;
                }
            }
        };
    });
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseExceptionHandler();
    app.UseStatusCodePages();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });
    app.MapControllers();
    app.MapHealthChecks("/health");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;