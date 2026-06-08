using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ResearchAssistant.UI.Services;

namespace ResearchAssistant.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly AgentStreamClient _client = new("http://localhost:5190");

    public ObservableCollection<ChatMessageViewModel> Messages { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string _inputText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private bool _isSending;

    private bool CanSend() => !IsSending && !string.IsNullOrWhiteSpace(InputText);

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(CancellationToken ct)
    {
        var question = InputText.Trim();
        InputText = string.Empty;
        IsSending = true;

        Messages.Add(new ChatMessageViewModel("You", question));

        var reply = new ChatMessageViewModel("Agent");
        Messages.Add(reply);

        try
        {
            await foreach (var evt in _client.StreamAsync(question, ct))
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    switch (evt.Type)
                    {
                        case "tool_call":
                            reply.ToolSteps.Add($"  {evt.Tool}: {evt.Input}");
                            break;
                        case "token":
                            reply.Content += evt.Token;
                            break;
                        case "error":
                            reply.Content += (reply.Content.Length > 0 ? "\n\n" : "") + $"Error: {evt.Token}";
                            break;
                    }
                });
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
                reply.Content = "Could not reach the agent. Is the API running?");
        }
        finally
        {
            await Dispatcher.UIThread.InvokeAsync(() => IsSending = false);
        }
    }
}
