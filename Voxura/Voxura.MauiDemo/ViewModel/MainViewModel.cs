using System.Text.Json;
using Voxura.Core;
using Voxura.MauiDemo.Model;

namespace Voxura.MauiDemo.ViewModel;

class MainViewModel :  BaseViewModel
{
    NLProcessing _nlp;

    Task<string>? _currentProcess;

    private RFQModel _rfqModel = new();

    public RFQModel RFQModel
    {
        get => _rfqModel;
        set => UpdateProperty(ref _rfqModel, value, nameof(RFQModel));
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

    public MainViewModel(ApplicationConfig appConfig)
    {
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
                Debug = currentText +  " -> " + task.Result;
            }
            catch (Exception ex)
            {
                Debug = ex.Message + "\n\n" + task.Result;
            }

            Task.Delay(500).ContinueWith(_ => ProcessTranscriptIfChanged(), TaskScheduler.FromCurrentSynchronizationContext());
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
