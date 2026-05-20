# Research Assistant

A local AI-powered research assistant built with C# to learn and demonstrate Generative AI, Vector Databases, RAG, Agentic AI, MCP, and ML.NET.

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

**3. Set up the database**

Connect to SQL Server on `localhost:1433` with `sa` / `Research@Assistant123` and run:
- `docker/sql/01_schema.sql`
- `docker/sql/02_seed.sql`

**4. Run the API**
```bash
dotnet run --project src/ResearchAssistant.Api/ResearchAssistant.Api.csproj
```

API runs on `http://localhost:5190`

## Configuration

All config in `appsettings.json`. Change `AI:Provider` from `Ollama` to `AzureOpenAI` to switch to cloud.

## Backlog
- [ ] Agent response should indicate source (knowledge base vs training knowledge)
- [ ] Azure OpenAI provider implementation
- [ ] gRPC endpoints
- [ ] ML.NET document classifier
- [ ] Avalonia UI