# Research Assistant

A local AI-powered helpdesk assistant built with C# to learn and demonstrate Generative AI, Vector Databases, RAG, Agentic AI, MCP, and ML.NET.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Ollama](https://ollama.com)

## Getting Started

**1. Install Ollama models**
```bash
ollama pull mistral:7b
ollama pull nomic-embed-text
```

**2. Start infrastructure**
```bash
cd docker && docker compose up -d
```

SQL schema and seed data are applied automatically on first start.

**3. Run the API**
```bash
dotnet run --project src/ResearchAssistant.Api/ResearchAssistant.Api.csproj
```

**4. Run the UI**
```bash
dotnet run --project src/ResearchAssistant.UI/ResearchAssistant.UI.csproj
```

API runs on `http://localhost:5190`

## Configuration

All config in `appsettings.json`. Change `AI:Provider` from `Ollama` to `AzureOpenAI` to switch to cloud (not yet implemented).

## Backlog
- [ ] Azure OpenAI provider implementation
- [ ] Source attribution (knowledge base vs model training data)
- [ ] gRPC endpoints
