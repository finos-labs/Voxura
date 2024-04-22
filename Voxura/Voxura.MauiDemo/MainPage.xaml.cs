using System.Text.Json;
using System.Threading;
using System.Globalization;
using CommunityToolkit.Maui.Media;

using Voxura.Core;
using Voxura.MauiDemo.Models;
using Voxura.MauiDemo.ViewModels;

using Voxura.MauiDemo.Platforms.SpeechToText;
using Voxura.MauiDemo.Views;

namespace Voxura.MauiDemo;

public partial class MainPage : ContentPage
{
    private CancellationTokenSource _cancellationTokenSource = new();
    private ISpeechToText _speechToText;
    private string _previousUserText = "";

    public MainPage()
    {
#if ANDROID
        _speechToText = new Voxura.MauiDemo.Platforms.SpeechToText.SpeechToTextImplementation();
#else
        _speechToText = SpeechToText.Default;
#endif
        InitializeComponent();
        BindingContext = new MainViewModel(new ApplicationConfig());
        RFQFormView.BindingContext = ((MainViewModel)BindingContext).RFQForm;

    }

    private async void StartDictation()
    {
        await ToggleListening(_cancellationTokenSource.Token);
    }

    private void DictateButton_Click(object sender, EventArgs e)
    {
        StartDictation();
    }

    private void UserText_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.UserText = UserText.Text; // needed in addition to binding to get quicker updates
        }
    }

    private async void OnListenClicked(object sender, EventArgs e)
    {
        await ToggleListening(_cancellationTokenSource.Token);
    }

    // TODO: separate it to SpeechToTextLogic
    private async Task ToggleListening(CancellationToken cancellationToken)
    {
        if (_speechToText.CurrentState == SpeechToTextState.Listening)
        {
            await StopListening(cancellationToken);
        }
        else
        {
            await StartListening(cancellationToken);
        }
    }

    private async Task StartListening(CancellationToken cancellationToken, string? buttonText = null)
    {
        _previousUserText = UserText.Text;
        var isGranted = await _speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            // Debug = "Permission not granted";
            return;
        }

        _speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
        await _speechToText.StartListenAsync(CultureInfo.CurrentCulture, cancellationToken);
    }

    private async Task StopListening(CancellationToken cancellationToken)
    {
        await _speechToText.StopListenAsync(cancellationToken);
        _speechToText.RecognitionResultUpdated -= OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted -= OnRecognitionTextCompleted;
    }

    private void OnRecognitionTextUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)
    {
        UserText.Text = _previousUserText + " " + args.RecognitionResult;
    }

    private void OnRecognitionTextCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
    {
        UserText.Text = _previousUserText + " " + args.RecognitionResult;
        _previousUserText = UserText.Text + ". ";
    }

}
