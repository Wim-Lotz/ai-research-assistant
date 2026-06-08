using System.Runtime.CompilerServices;
using OpenAI.Chat;

namespace ResearchAssistant.Api.Infrastructure;

public class AzureOpenAILanguageModelService : ILanguageModelService
{
    private readonly ChatClient _chatClient;

    public AzureOpenAILanguageModelService(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        var response = await _chatClient.CompleteChatAsync(
            [new UserChatMessage(prompt)],
            cancellationToken: ct);

        return response.Value.Content[0].Text;
    }

    public async IAsyncEnumerable<string> StreamAsync(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var update in _chatClient.CompleteChatStreamingAsync(
            [new UserChatMessage(prompt)],
            cancellationToken: ct))
        {
            foreach (var part in update.ContentUpdate)
            {
                if (part.Text is not null)
                    yield return part.Text;
            }
        }
    }
}
