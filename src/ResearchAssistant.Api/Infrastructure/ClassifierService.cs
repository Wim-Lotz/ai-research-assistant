using Microsoft.ML;
using Microsoft.ML.Data;

namespace ResearchAssistant.Api.Infrastructure;

public class ClassifierService : IClassifierService
{
    private readonly PredictionEngine<InputSchema, OutputSchema>? _predictionEngine;

    public ClassifierService()
    {
        var modelPath = Path.Combine(AppContext.BaseDirectory, "Data", "helpdesk-classifier.zip");

        if (!File.Exists(modelPath))
            return;

        var context = new MLContext();
        var model = context.Model.Load(modelPath, out _);
        _predictionEngine = context.Model.CreatePredictionEngine<InputSchema, OutputSchema>(model, ignoreMissingColumns: true);
    }

    public string Predict(string text)
    {
        if (_predictionEngine is null)
            return "unknown";

        return _predictionEngine.Predict(new InputSchema { Text = text }).PredictedType;
    }

    private class InputSchema
    {
        public string Label { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    private class OutputSchema
    {
        [ColumnName("PredictedLabel")]
        public string PredictedType { get; set; } = string.Empty;
    }
}
