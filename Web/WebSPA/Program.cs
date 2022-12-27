using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();


var logstashUrl = builder.Configuration["Serilog:LogstashUrl"];
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.WithProperty("ApplicationContext", typeof(Program).Namespace)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:5044" : logstashUrl, queueLimitBytes: null)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();


var services = builder.Services;
services.AddHealthChecks();
services.AddControllers();

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();