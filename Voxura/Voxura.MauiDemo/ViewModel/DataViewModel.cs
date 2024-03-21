using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Voxura.Core;
using Voxura.MauiDemo.Model;

namespace Voxura.MauiDemo.ViewModel;

class DataViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    NLProcessing _nlp;

    Task<string>? _currentProcess;

    private RFQ _rfq = new();

    private string? _transcript;
    public string? Transcript
    {
        get => _transcript;
        set => UpdateTranscript(value);
    }

    private string? _debug;
    public string Debug
    {
        get => _debug;
        set => UpdateProperty(ref _debug, value, nameof(Debug));
    }

    private string? _status;
    public string Status
    {
        get => _status;
        set => UpdateProperty(ref _status, value, nameof(Status));
    }

    private string? _interimTranscript;
    public string InterimTranscript
    {
        get => _interimTranscript;
        set => UpdateProperty(ref _interimTranscript, value, nameof(InterimTranscript));
    }

    public string Email
    {
        get => _rfq.Requestor?.Id?.Email ?? string.Empty;
        set
        {
            if (_rfq.Requestor?.Id?.Email != value)
            {
                PrepareRequestorIfNeeded();
                _rfq.Requestor.Id.Email = value;
                OnPropertyChanged();
            }
        }
    }

    public string Name
    {
        get => _rfq.Requestor?.Name ?? string.Empty;
        set
        {
            if (_rfq.Requestor?.Name != value)
            {
                PrepareRequestorIfNeeded();
                _rfq.Requestor.Name = value;
                OnPropertyChanged();
            }

        }
    }

    public string? Direction
    {
        get => _rfq.Direction?.ToString();
        set
        {
            Enum.TryParse(value, out Direction enumVal);
            if (_rfq.Direction != enumVal)
            {
                _rfq.Direction = enumVal;
                OnPropertyChanged();
            }
        }
    }

    public string? RollConvention
    {
        get => _rfq.RollConvention?.ToString();
        set
        {
            Enum.TryParse(value, out RollConvention enumVal);
            if (_rfq.RollConvention != enumVal)
            {
                _rfq.RollConvention = enumVal;
                OnPropertyChanged();
            }
        }
    }

    public string? Product
    {
        get => _rfq.Trade?.Product;
        set
        {
            if (_rfq.Trade?.Product != value)
            {
                PrepareTradeIfNeeded();
                _rfq.Trade.Product = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Notes
    {
        get => _rfq.Notes;
        set
        {
            if (_rfq.Notes != value)
            {
                _rfq.Notes = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime? StartDate
    {
        get => _rfq.StartDate;
        set
        {
            if (_rfq.StartDate != value)
            {
                _rfq.StartDate = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime? EndDate
    {
        get => _rfq.EndDate;
        set
        {
            if (_rfq.EndDate != value)
            {
                _rfq.EndDate = value;
                OnPropertyChanged();
            }
        }
    }

    public int? Notional
    {
        get => _rfq.Notional;
        set
        {
            if (_rfq.Notional != value)
            {
                _rfq.Notional = value;
                OnPropertyChanged();
            }
        }

    }

    public DataViewModel()
    {
        var config = new NLProcessingConfig
        {
            ApiKey = "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",
            ExtractionPrompt = @"Below is a raw transcript of a user's verbal instructions to fill a form.
                                 Convert it to a JSON object that conforms to the TypeScript interface below.
                                 Ignore anything else. Answer only with the required object and nothing else !

                                interface Contact {
                                    Id?: {
                                        Email?: string as Email;
                                    };
                                    Name?: string;
                                }

                                interface Organization {
                                    Id?: {
                                        PERMID?: string;
                                    };
                                    Name?: string;
                                }

                                interface RFQ {
                                    Requestor?: Contact | Organization;
                                    Direction?: 'Buy' | 'Sell';
                                    Notional?: number as Int;
                                    StartDate?: date; // be strict with locale format or null
                                    EndDate?: date; // be strict with locale format or null
                                    RollConvention?: 'Following' | 'Modified Following' | 'Preceding';
                                    Trade?: {
                                        Product: string;  // The product or currency the user wants to buy or sell
                                    };
                                    Notes?: string;  // Any other information not captured by the above fields
                                }",
        };

        _nlp = new NLProcessing(config);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void UpdateTranscript(string value)
    {
        _transcript = value;
        ProcessTranscriptIfChanged();
    }

    private T UpdateProperty<T>(ref T value, T newValue, string name)
    {
        if (!EqualityComparer<T>.Default.Equals(value, newValue))
        {
            value = newValue;
            OnPropertyChanged(name);
        }

        return value;
    }

    private void PrepareTradeIfNeeded()
    {
        if (_rfq.Trade == null)
        {
            _rfq.Trade = new();
        }
    }

    private void PrepareRequestorIfNeeded()
    {
        if (_rfq.Requestor == null)
        {
            _rfq.Requestor = new();
        }

        if (_rfq.Requestor.Id == null)
        {
            _rfq.Requestor.Id = new();
        }
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
        Debug = currentText;

        _currentProcess = _nlp.ProcessAsync(currentText);
        _currentProcess.ContinueWith(task =>
        {
            _debug = task.Result;
            try
            {
                RFQ? myForm = JsonSerializer.Deserialize<RFQ>(task.Result);
                if (myForm != null)
                {
                    Email = myForm.Requestor?.Id?.Email ?? string.Empty;
                    Name = myForm.Requestor?.Name ?? string.Empty;
                    Direction = myForm.Direction.ToString();
                    Notional = myForm.Notional;
                    RollConvention = myForm.RollConvention.ToString();
                    Product = myForm.Trade.Product;
                    StartDate = myForm.StartDate;
                    EndDate = myForm.EndDate;
                    Notes = myForm.Notes;
                }
            }
            catch (Exception ex)
            {
                _debug = ex.Message + "\n\n" + task.Result;
            }

            OnPropertyChanged(nameof(Debug));
            Task.Delay(500).ContinueWith(_ => ProcessTranscriptIfChanged(), TaskScheduler.FromCurrentSynchronizationContext());
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}