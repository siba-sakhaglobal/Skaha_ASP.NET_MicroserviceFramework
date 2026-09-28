namespace SakhaERP.Shared.Logging;

using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

public static class SerilogConfiguration
{
    public static IHostBuilder UseSakhaSerilog(this IHostBuilder hostBuilder, string serviceName)
    {
        return hostBuilder.UseSerilog((context, config) =>
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj} {Properties:j}{NewLine}{Exception}");

            // Add App Insights sink in production
            // .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces);
        });
    }
}
