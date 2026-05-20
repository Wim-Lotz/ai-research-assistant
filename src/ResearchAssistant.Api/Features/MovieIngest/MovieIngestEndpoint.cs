using FastEndpoints;
using Microsoft.Data.SqlClient;
using ResearchAssistant.Api.Infrastructure;

namespace ResearchAssistant.Api.Features.MovieIngest;

public class MovieIngestResponse
{
    public int MoviesIngested { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class MovieIngestEndpoint : EndpointWithoutRequest<MovieIngestResponse>
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IDocumentService _documentService;
    private readonly IConfiguration _configuration;

    public MovieIngestEndpoint(
        IEmbeddingService embeddingService,
        IDocumentService documentService,
        IConfiguration configuration)
    {
        _embeddingService = embeddingService;
        _documentService = documentService;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Post("/ingest/movies");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await _documentService.InitialiseAsync(ct);

        var connectionString = _configuration["SqlServer:ConnectionString"];
        var movies = new List<(string Id, string Text)>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(ct);

        var sql = """
                  SELECT 
                      m.Id,
                      m.Title,
                      m.Year,
                      m.Rating,
                      m.Plot,
                      d.Name AS Director,
                      (SELECT STRING_AGG(a.Name, ', ') FROM MovieActors ma JOIN Actors a ON ma.ActorId = a.Id WHERE ma.MovieId = m.Id) AS Actors,
                      (SELECT STRING_AGG(g.Name, ', ') FROM MovieGenres mg JOIN Genres g ON mg.GenreId = g.Id WHERE mg.MovieId = m.Id) AS Genres
                  FROM Movies m
                  JOIN MovieDirectors md ON m.Id = md.MovieId
                  JOIN Directors d ON md.DirectorId = d.Id
                  """;

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            var id = reader["Id"].ToString()!;
            var text = $"""
                Title: {reader["Title"]}
                Year: {reader["Year"]}
                Rating: {reader["Rating"]}
                Director: {reader["Director"]}
                Actors: {reader["Actors"]}
                Genres: {reader["Genres"]}
                Plot: {reader["Plot"]}
                """;

            movies.Add((Guid.NewGuid().ToString(), text));
        }

        foreach (var (id, text) in movies)
        {
            var embedding = await _embeddingService.EmbedAsync(text, ct);
            await _documentService.StoreAsync(id, text, embedding, ct);
        }

        await Send.OkAsync(new MovieIngestResponse
        {
            MoviesIngested = movies.Count,
            Message = $"Successfully ingested {movies.Count} movies into vector database."
        }, ct);
    }
}