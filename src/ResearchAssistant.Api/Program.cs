using ResearchAssistant.Api.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();
builder.AddFastEndpointsConfiguration();
builder.Services.AddOpenApi();
builder.AddAIConfiguration();
builder.AddQdrantConfiguration();
builder.AddMcpConfiguration();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseFastEndpointsConfiguration();

app.Run();