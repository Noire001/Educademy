using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logstashUrl = builder.Configuration["Serilog:LogstashUrl"];
var name = Assembly.GetEntryAssembly()!.GetName().Name;

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("ApplicationContext", name)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://elk:5045" : logstashUrl, queueLimitBytes: null)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();
var services = builder.Services;
services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
services.AddHealthChecksUI().AddInMemoryStorage();
services.AddControllers();
var app = builder.Build();
var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}
app.UseHealthChecksUI(config =>
{
    config.ResourcesPath = string.IsNullOrEmpty(pathBase) ? "/ui/resources" : $"{pathBase}/ui/resources";
    config.UIPath = "/hc-ui";
});
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
await app.RunAsync();