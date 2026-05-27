namespace ResearchAssistant.Api.Infrastructure;

public interface IClassifierService
{
    string Predict(string text);
}
