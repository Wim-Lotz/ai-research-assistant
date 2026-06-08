using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ResearchAssistant.UI.ViewModels;

public partial class ChatMessageViewModel : ObservableObject
{
    public string Role { get; }

    [ObservableProperty]
    private string _content = string.Empty;

    public ObservableCollection<string> ToolSteps { get; } = [];

    public ChatMessageViewModel(string role, string initialContent = "")
    {
        Role = role;
        _content = initialContent;
    }
}
