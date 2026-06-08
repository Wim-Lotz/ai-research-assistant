using Avalonia.Controls;
using Avalonia.Input;
using ResearchAssistant.UI.ViewModels;

namespace ResearchAssistant.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        var vm = (MainWindowViewModel)DataContext!;
        if (vm.SendCommand.CanExecute(null))
            vm.SendCommand.Execute(null);
    }
}
