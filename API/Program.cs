using Serilog;
using API;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information($"Starting application: {typeof(Program).Assembly.GetName().Name}");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    await builder
        .ConfigureServices()
        .Build()
        .ConfigurePipeline()
        .RunAsync();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
