using FastEndpoints;

namespace ResearchAssistant.Api.Configuration;

public static class FastEndpointsConfiguration
{
    public static void AddFastEndpointsConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddFastEndpoints();
    }

    public static void UseFastEndpointsConfiguration(this WebApplication app)
    {
        app.UseFastEndpoints();
    }
}