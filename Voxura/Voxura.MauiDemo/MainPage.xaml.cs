using Voxura.Core;
using Voxura.MauiDemo.Model;
using Voxura.MauiDemo.ViewModel;
using System.Text.Json;
using System.Threading;

using CommunityToolkit.Maui.Media;

namespace Voxura.MauiDemo;

public partial class MainPage : ContentPage
{
    CancellationTokenSource _cancellationTokenSource = new();
    public MainPage()
    {
#if ANDROID
        var speechToText = new Voxura.MauiDemo.Platforms.SpeechToText.SpeechToTextImplementation();
#else
        var speechToText = SpeechToText.Default;
#endif
        InitializeComponent();
        BindingContext = new MainViewModel(new ApplicationConfig(),  speechToText);
    }

    private void OnDebugModeToggled(object sender, ToggledEventArgs e)
    {
        Debug.IsVisible = e.Value;
        InterimTranscript.IsVisible = e.Value;
    }

    private async void OnListenClicked(object sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            await vm.ToggleListening(_cancellationTokenSource.Token);
        }
    }
}
