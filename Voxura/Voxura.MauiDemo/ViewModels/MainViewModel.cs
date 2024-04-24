using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Voxura.Core;

using Voxura.MauiDemo.Models;
using Contact = Voxura.MauiDemo.Models.Contact;

namespace Voxura.MauiDemo.ViewModels;

public class MainViewModel : ObservableObject
{
    public RFQFormViewModel RFQForm { get; } = new();

    private ApplicationConfig _applicationConfig;
    private NLProcessing _nlProcessing;

    /// <summary>
    /// Prompt to use for extraction <see cref="NLProcessingConfig"/>
    /// </summary>
    public string Prompt { get; set; } =
        @"Below is a raw transcript of a user's verbal instructions to fill a form.
Convert it to a JSON object that conforms to the TypeScript interface below.
Ignore anything else. Answer only with the required object and nothing else!

interface Contact {
    Id?: {
        Email?: string; // only valid email address or null
    };
    Name?: string;
}

interface RFQ {
    Requestor?: Contact;
    Direction?: 'Buy' | 'Sell';
    Notional?: number as Int;
    StartDate?: date; // be strict with locale format or null
    EndDate?: date; // be strict with locale format or null
    RollConvention?: 'Following' | 'ModifiedFollowing' | 'Preceding';
    Trade?: {
        Product: string;  // The product or currency the user wants to buy or sell
    };
    Notes?: string;  // Any other information not captured by the above fields
};

";

    public MainViewModel(ApplicationConfig appConfig)
    {
        _applicationConfig = appConfig;
        Initialize(false);
    }

    public void Initialize(bool designMode = true)
    {
        if (designMode)
        {
            FillDesignData();
        }
    }

    private string? _userText = string.Empty;

    /// <summary>
    /// Gets or sets the user text.
    /// </summary>
    public string? UserText
    {
        get => _userText;
        set
        {
            if (SetProperty(ref _userText, value))
                ProcessNewUserText();
        }
    }

    private bool _isProcessing = false;
    private bool _pendingChanges;

    private async Task ProcessNewUserText()
    {
        if (!EnsureNlProcessing())
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await Toast.Make("API key is not set, please update in Settings!", ToastDuration.Long, 16).Show(cancellationTokenSource.Token);
            return;
        }

        if (_isProcessing)
        {
            _pendingChanges = true;
            return;
        }

        try
        {
            _isProcessing = true;
            var result = await _nlProcessing.ProcessAsync(UserText);
            RFQ? rfq = JsonSerializer.Deserialize<RFQ>(result);
            if (rfq != null)
            {
                RFQForm.SetFromRFQ(rfq);
            }
        }
        finally
        {
            _isProcessing = false;

            if (_pendingChanges)
            {
                _pendingChanges = false;
                await ProcessNewUserText();
            }
        }
    }

    private void FillDesignData()
    {
        var rfq = new RFQ
        {
            Requestor = new Contact
            {
                Name = "John Doe",
                Id = new EmailId
                {
                    Email = "john.doe@abc.com"
                }
            },
            Direction = Direction.Sell,
            StartDate = new DateTime(2025, 01, 02),
            EndDate = new DateTime(2025, 07, 02),
            Notional = 30000,
            Notes = "Here are some notes for you",
            RollConvention = RollConvention.Preceding,
            Trade = new Trade
            {
                Product = "USD"
            }
        };

        RFQForm.SetFromRFQ(rfq);
    }

    private bool EnsureNlProcessing()
    {
        if (_nlProcessing == null)
        {
            if (!_applicationConfig.Verify())
            {
                return false;
            }

            NLProcessingConfig config = new()
            {
                ApiKey = _applicationConfig.ApiKey,
                OpenAIKeyLoadFromEnvironment = _applicationConfig.OpenAIKeyLoadFromEnvironment,
                ExtractionPrompt = Prompt,
                ModelName = _applicationConfig.ModelName,
            };

            _nlProcessing = new NLProcessing(config);
        }

        return true;
    }
}
