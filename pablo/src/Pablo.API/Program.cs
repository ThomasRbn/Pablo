using Pablo.API.Exceptions;
using Pablo.API.Features.Auth.Services;
using Pablo.API.Infrastructure;

using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // preserveStaticLogger: WebApplicationFactory builds multiple hosts in one process;
    // CreateBootstrapLogger + freeze would throw "The logger is already frozen."
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(),
        preserveStaticLogger: true);

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddScoped<IAuthService, AuthService>();
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
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            // Let feature services own required-field validation (e.g. InvalidLoginRequestException).
            // Otherwise [ApiController] returns a generic 400 before the action runs when JSON
            // sends null for non-nullable strings.
            options.SuppressModelStateInvalidFilter = true;
        });
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