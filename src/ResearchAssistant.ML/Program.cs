using System.Text.Json;
using Microsoft.ML;
using Microsoft.ML.Data;

var dataFolder = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "../../../../ResearchAssistant.Api/Data"));

var seedPath = Path.Combine(dataFolder, "helpdesk-documents.json");
var modelPath = Path.Combine(dataFolder, "helpdesk-classifier.zip");

var json = await File.ReadAllTextAsync(seedPath);
var seed = JsonSerializer.Deserialize<SeedData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

var trainingRecords = seed.Documents
    .Select(d => new DocumentInput
    {
        Label = d.Type,
        Text = $"{d.Title}\n\n{d.Content}"
    })
    .ToList();

Console.WriteLine($"Training on {trainingRecords.Count} documents...");

var context = new MLContext(seed: 42);
var dataView = context.Data.LoadFromEnumerable(trainingRecords);

var pipeline = context.Transforms.Conversion.MapValueToKey("Label")
    .Append(context.Transforms.Text.FeaturizeText("Features", nameof(DocumentInput.Text)))
    .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy())
    .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

var model = pipeline.Fit(dataView);

var predictions = model.Transform(dataView);
var metrics = context.MulticlassClassification.Evaluate(predictions);

Console.WriteLine($"Macro accuracy : {metrics.MacroAccuracy:P2}");
Console.WriteLine($"Micro accuracy : {metrics.MicroAccuracy:P2}");
Console.WriteLine($"Log-loss       : {metrics.LogLoss:F4}");

context.Model.Save(model, dataView.Schema, modelPath);
Console.WriteLine($"Model saved to {modelPath}");

class DocumentInput
{
    public string Label { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

class SeedData
{
    public List<SeedDocument> Documents { get; init; } = [];
}

class SeedDocument
{
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}
