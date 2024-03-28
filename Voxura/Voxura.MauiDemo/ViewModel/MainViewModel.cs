using System.Text.Json;
using Voxura.Core;
using Voxura.MauiDemo.Model;
using CommunityToolkit.Maui.Media;
using System.Globalization;

namespace Voxura.MauiDemo.ViewModel;

class MainViewModel : BaseViewModel
{
    NLProcessing _nlp;

    Task<string>? _currentProcess;

    private readonly ISpeechToText _speechToText;

    private RFQModel _rfqModel = new();

    public RFQModel RFQModel
    {
        get => _rfqModel;
        set => UpdateProperty(ref _rfqModel, value);
    }

    private string? _transcript;
    public string? Transcript
    {
        get => _transcript;
        set => UpdateTranscript(value);
    }

    private string _debug = "";
    public string Debug
    {
        get => _debug;
        set => UpdateProperty(ref _debug, value, nameof(Debug));
    }


    private string _listeningButtonText = "Listen";
    public string ListeningButtonText
    {
        get => _listeningButtonText;
        set => UpdateProperty(ref _listeningButtonText, value, nameof(ListeningButtonText));
    }

    private string _status = "";
    public string Status
    {
        get => _status;
        set => UpdateProperty(ref _status, value, nameof(Status));
    }

    private string _interimTranscript = "";
    public string InterimTranscript
    {
        get => _interimTranscript;
        set => UpdateProperty(ref _interimTranscript, value, nameof(InterimTranscript));
    }

    public MainViewModel(ApplicationConfig appConfig, ISpeechToText speechToText)
    {
        _speechToText = speechToText;
        var config = new NLProcessingConfig
        {
            ApiKey = appConfig.ApiKey,
            OpenAIKeyLoadFromEnvironment = appConfig.OpenAIKeyLoadFromEnvironment,
            ExtractionPrompt = appConfig.ExtractionPrompt + "\n" + appConfig.ExpectedOutput,
        };

        _nlp = new NLProcessing(config);
    }

    private void UpdateTranscript(string? value)
    {
        _transcript = value;
        ProcessTranscriptIfChanged();
    }


    private void ProcessTranscriptIfChanged()
    {
        bool isProcessing = _currentProcess != null && !_currentProcess.IsCompleted;
        Status = !isProcessing ? "Completed " : "Processing... ";
        if (isProcessing)
        {
            return;
        }

        string currentText = _transcript ?? string.Empty;
        if (_interimTranscript?.Trim() == currentText.Trim() || currentText.Length < 10)
        {
            return;
        }

        InterimTranscript = currentText;

        _currentProcess = _nlp.ProcessAsync(currentText);
        _currentProcess.ContinueWith(task =>
        {
            try
            {
                RFQ? myForm = JsonSerializer.Deserialize<RFQ>(task.Result);
                if (myForm != null)
                {
                    RFQModel.RFQ = myForm;
                }
                Debug = currentText + " -> " + task.Result;
            }
            catch (Exception ex)
            {
                Debug = ex.Message + "\n\n" + task.Result;
            }

            Task.Delay(500).ContinueWith(_ => ProcessTranscriptIfChanged(), TaskScheduler.FromCurrentSynchronizationContext());
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public async Task ToggleListening(CancellationToken cancellationToken)
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

    private async Task StartListening(CancellationToken cancellationToken)
    {
        var isGranted = await _speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            Debug = "Permission was not granted";
            return;
        }

        _speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
        await SpeechToText.StartListenAsync(CultureInfo.CurrentCulture, CancellationToken.None);
        ListeningButtonText = "Stop listening";
    }

    private async Task StopListening(CancellationToken cancellationToken)
    {
        await SpeechToText.StopListenAsync(CancellationToken.None);
        SpeechToText.Default.RecognitionResultUpdated -= OnRecognitionTextUpdated;
        SpeechToText.Default.RecognitionResultCompleted -= OnRecognitionTextCompleted;
        ListeningButtonText = "Listen";
    }

    void OnRecognitionTextUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)
    {
        UpdateTranscript(_transcript + args.RecognitionResult);

    }

    void OnRecognitionTextCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
    {
        UpdateTranscript(args.RecognitionResult);
    }
}
