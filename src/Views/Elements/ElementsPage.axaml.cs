using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ClassicDiagnostics.Avalonia.Elements;

namespace ClassicDiagnostics.Avalonia.Views.Elements;

internal partial class ElementsPage : ReactiveUserControl<ElementsPageViewModel>
{
    private readonly TextBox? _findTextBox;

    public ElementsPage(ElementsPageViewModel viewModel) : base(viewModel)
    {
        AvaloniaXamlLoader.Load(this);
        _findTextBox = this.FindNameScope()?.Find<TextBox>("FindTextBox");
        AddHandler(KeyDownEvent, HandleTargetKeyDown, RoutingStrategies.Tunnel);
    }

    private void HandleFindNextClick(object? sender, RoutedEventArgs e)
    {
        RequiredViewModel.Find.FindNext();
    }

    private void HandleFindPreviousClick(object? sender, RoutedEventArgs e)
    {
        RequiredViewModel.Find.FindPrevious();
    }

    private void HandleTargetKeyDown(object? sender, KeyEventArgs e)
    {
        var viewModel = RequiredViewModel;

        if (e.Key == Key.F && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            viewModel.Find.IsVisible = !viewModel.Find.IsVisible;
            if (viewModel.Find.IsVisible)
            {
                Dispatcher.UIThread.Post(
                    () =>
                    {
                        _findTextBox?.Focus();
                        _findTextBox?.SelectAll();
                    },
                    DispatcherPriority.Background);
            }

            e.Handled = true;
            return;
        }

        if (!viewModel.Find.IsVisible)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Escape:
                viewModel.Find.IsVisible = false;
                e.Handled = true;
                break;
            case Key.Enter when e.KeyModifiers.HasFlag(KeyModifiers.Shift):
                viewModel.Find.FindPrevious();
                e.Handled = true;
                break;
            case Key.Enter:
                viewModel.Find.FindNext();
                e.Handled = true;
                break;
        }
    }
}