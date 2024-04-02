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

    private string _previousTranscript = "";
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

    private string _savedInterimText = "";
    private string? _savedRfq = null;

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
            // now we can save the current state
            _savedInterimText = currentText;
            _savedRfq = JsonSerializer.Serialize(RFQModel.RFQ);
            return;
        }

        InterimTranscript = currentText;
        if (_savedRfq != null)
        {
            currentText = "I have the following state, please update it with the text bellow: \n" + _savedRfq + "\n"
                + currentText.Substring(_savedInterimText.Length);
        }

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

    private async Task StartListening(CancellationToken cancellationToken, string? buttonText = null)
    {
        _previousTranscript = _transcript;
        var isGranted = await _speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            Debug = "Permission not granted";
            return;
        }

        _speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
        await _speechToText.StartListenAsync(CultureInfo.CurrentCulture, cancellationToken);
        ListeningButtonText = buttonText ?? "Stop listening";
    }

    private async Task StopListening(CancellationToken cancellationToken)
    {
        await _speechToText.StopListenAsync(CancellationToken.None);
        _speechToText.RecognitionResultUpdated -= OnRecognitionTextUpdated;
        _speechToText.RecognitionResultCompleted -= OnRecognitionTextCompleted;
        ListeningButtonText = "Listen";
    }

    void OnRecognitionTextUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)
    {
        UpdateTranscript(_previousTranscript + " " + args.RecognitionResult);
        OnPropertyChanged(nameof(Transcript));
    }

    void OnRecognitionTextCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
    {

        UpdateTranscript(_previousTranscript + " " + args.RecognitionResult);
        OnPropertyChanged(nameof(Transcript));

        _previousTranscript = _transcript + " ";
    }
}
