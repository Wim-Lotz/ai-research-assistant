# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Developer Profile

- Senior C# backend developer — strong .NET background, no need to explain language basics.
- New to AI concepts — explain AI patterns, terminology, and design decisions thoroughly.
- Prefers to understand every line of code — do not generate large blocks without explanation.
- No vibe coding — code quality and understanding over speed.
- Self-documenting code — no comments or XML docs needed.

## Setup Philosophy

**The entire project must be runnable with only Docker Desktop and the .NET SDK installed.** No manual database steps, no pre-configured external services, no undocumented prerequisites.

This means:
- All infrastructure (Qdrant, SQL Server, MCP server, DAB) runs in Docker Compose
- SQL schema and seed data execute automatically on first `docker compose up` — never require manual script execution
- Ollama model pulls are documented as a single copy-paste block
- The README must be good enough that someone who has never seen the project can clone it and have it running within minutes

## Project Purpose

This is a teaching and portfolio project. The goal is to learn the full AI engineering stack to a level where the developer can help companies implement AI solutions. Breadth of AI concepts matters more than polish. Always explain *why* a pattern exists and what problem it solves in a real company context, not just how to implement it.

**Domain:** A company IT helpdesk (NexusSupport). Structured data (tickets, employees, customers) lives in SQL Server. Unstructured data (resolution guides, procedures) lives in Qdrant. The agent queries both to answer helpdesk questions.

**What has been learned so far:**
- **RAG** — the most common pattern companies ask for: "we have documents, we want to query them with AI."
- **ReAct agentic loops** — how you go from a single LLM call to an AI that can reason and act over multiple steps.
- **MCP Server** — the emerging standard for giving agents access to tools and data sources. Teaches structured SQL queries from an agent, and hybrid retrieval when combined with Qdrant semantic search.
- **Unstructured data ingestion** — ingesting helpdesk documents into Qdrant to enable semantic search over resolution procedures.
- **ML.NET classifier** — classifying ingested documents on their way into Qdrant so semantic search can be scoped by document type.
- **Avalonia UI** — desktop chat interface with streaming SSE responses, showing tool steps in real time.

**What each remaining roadmap item teaches:**
- **Azure OpenAI** — swapping Ollama for the provider enterprises actually use. Teaches provider abstraction and cloud LLM integration.
- **Source attribution** — a real enterprise requirement: did this answer come from the knowledge base or the model's training data? Relevant for compliance and audit trails.

**The motivating query** (what this stack answers): *"Which open high-priority tickets have been unresolved the longest, and what does our knowledge base say about resolving that category of issue?"* This requires SQL (ticket status, priority, age) + semantic search (resolution procedures) working together through the agent.

**Build order:** MCP Server → unstructured data ingestion → ML.NET classifier → Azure OpenAI → Avalonia UI → source attribution.

## Tech Stack

C# 12 / .NET 10 REST API using **FastEndpoints** (not minimal APIs or controllers). Semantic search via **Qdrant** (vector DB), structured data via **SQL Server 2022**, local LLM inference via **Ollama**. Microsoft.Extensions.AI provides the LLM abstraction layer.

## Required Setup

Infrastructure (Qdrant + SQL Server + DAB/MCP) runs in Docker — schema and seed data are applied automatically on first start:

```bash
cd docker && docker compose up -d
```

Pull Ollama models before first run:

```bash
ollama pull mistral:7b
ollama pull nomic-embed-text
```

## Running the API

```bash
dotnet run --project src/ResearchAssistant.Api/ResearchAssistant.Api.csproj
```

API listens on `http://localhost:5190`.

## Running the UI

```bash
dotnet run --project src/ResearchAssistant.UI/ResearchAssistant.UI.csproj
```

## Architecture

Vertical slice architecture under `/Features`. Each feature folder contains everything for that feature (endpoint, request, response, any feature-specific services). Shared AI infrastructure lives in `/Infrastructure`.

**Do not** reorganise into Clean Architecture layers (no `/Controllers`, `/Services`, `/Repositories` folders).

## What Has Been Built

- GET `/health` — health check
- POST `/generate` — raw LLM generation
- POST `/ingest` — ingest text into Qdrant
- POST `/ingest/helpdesk` — ingest helpdesk documents from JSON into Qdrant
- POST `/search` — semantic search against Qdrant
- POST `/rag` — full RAG pipeline (embed → search → augment → generate)
- POST `/agent` — ReAct agentic loop with tool calling (returns complete response)
- POST `/agent/stream` — ReAct agentic loop with SSE streaming (yields tool steps + answer tokens)

## Remaining Roadmap

1. **Source attribution** — indicate whether answer came from knowledge base or model training data

## Backlog

- Clean old test documents from Qdrant (machine learning, vector db, docker text entries)

## Data

### SQL Server — NexusSupport
Structured helpdesk data: Departments, Employees, Customers, Tickets. 50 tickets across 10 customers and 15 employees. Queried via MCP/DAB using OData filter expressions.

### Qdrant — helpdesk documents
Unstructured resolution guides and procedures, classified by document type via ML.NET. Queried via semantic search.

### Why both stores
SQL handles exact structured queries ("open critical tickets assigned to James Crawford"). Qdrant handles semantic queries ("how do we handle ransomware incidents?"). The agent uses both and combines the results.

## Switching AI Provider

Set `AI:Provider` in `appsettings.json` to `"Ollama"` (default) or `"AzureOpenAI"`.

For Azure OpenAI, also set:
- `AzureOpenAI:Endpoint` — your resource endpoint (`https://<resource>.openai.azure.com/`)
- `AzureOpenAI:DeploymentName` — your chat model deployment name (e.g. `gpt-4o`)
- `AzureOpenAI:ApiKey` — store this in user secrets, not appsettings: `dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-key>"`

Embeddings always use Ollama (`nomic-embed-text`). Switching embedding providers would require re-ingesting all documents into Qdrant with the new vector dimensions.

## Code Style

- Use `record` or `record struct` for request/response DTOs and value objects; use `class` for services.
- Do not add comments or XML docs — code should be self-documenting via naming.
- Nullable reference types are enabled; handle nullability explicitly.
- All I/O must be async with `CancellationToken` propagation.

## Commits

Follow Conventional Commits: `feat:`, `fix:`, `chore:`, `refactor:`, `docs:`, `test:`.

## Gotchas

- **Vector size is hardcoded to 768** (`nomic-embed-text` dimensions) in `IEmbeddingService`/Qdrant setup. Changing embedding models requires updating this constant.
- **Agent loops cap at 3 iterations** (`AgentRunner`). If the LLM doesn't converge within 3 tool calls the loop stops and returns a partial answer.
- **No authentication** — all endpoints use `AllowAnonymous`. Do not expose the API publicly.
- There are no tests in this repo.
