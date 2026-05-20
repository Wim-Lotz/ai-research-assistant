using Serilog;

namespace ResearchAssistant.Api.Configuration;

public static class SerilogConfiguration
{
    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }
}