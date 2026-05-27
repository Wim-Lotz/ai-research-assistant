using ResearchAssistant.Api.Configuration;
using ResearchAssistant.Api.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();
builder.AddFastEndpointsConfiguration();
builder.Services.AddOpenApi();
builder.AddAIConfiguration();
builder.AddQdrantConfiguration();
builder.AddMcpConfiguration();
builder.Services.AddSingleton<IClassifierService, ClassifierService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseFastEndpointsConfiguration();

app.Run();