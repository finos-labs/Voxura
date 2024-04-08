using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenAI;
using Voxura.Core;

namespace Voxura.WpfDemo.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public RFQFormViewModel RFQForm { get; } = new RFQFormViewModel();
        
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

        public MainViewModel()
        {
            NLProcessingConfig config = new();
            config.ExtractionPrompt = Prompt;
            config.OpenAIKeyLoadFromEnvironment = true;
            config.ModelName = "gpt-3.5-turbo";
            
            _nlProcessing = new NLProcessing(config);
        }
        
        public void Initialize(bool designMode = true)
        {
            if (designMode)
            {
                FillDesignData();
            }
        }
        
        private string _userText;

        public string UserText
        {
            get => _userText;
            set
            {
                if (SetProperty(ref _userText, value))
                    ProcessNewUserText();
            }
        }


        private bool _isProcessing;
        private bool _pendingChanges;
        
        private async Task ProcessNewUserText()
        {
            if (_isProcessing)
            {
                _pendingChanges = true;
                return;
            }

            try
            {
                _isProcessing = true;

                var result = await _nlProcessing.ProcessAsync(UserText);

                Debug.WriteLine(result);

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
            var rfq = new RFQ();
            rfq.Requestor = new Contact();
            rfq.Requestor.Name = "John Doe";
            rfq.Requestor.Id = new EmailId();
            rfq.Requestor.Id.Email = "john.doe@abc.com";
            rfq.Direction = Direction.Sell;
            rfq.StartDate = new DateTime(2025, 01, 02);
            rfq.EndDate = new DateTime(2025, 07, 02);
            rfq.Notional = 30000;
            rfq.Notes = "Here are some notes for you";
            rfq.RollConvention = RollConvention.Preceding;
            rfq.Trade = new Trade();
            rfq.Trade.Product = "USD";

            RFQForm.SetFromRFQ(rfq);
        }
    }
}
