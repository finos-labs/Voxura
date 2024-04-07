using Voxura.MauiDemo.Model;
using Contact = Voxura.MauiDemo.Model.Contact;

namespace Voxura.MauiDemo.ViewModel;

class RFQModel : BaseViewModel
{
    static private string _defaultBackground = "Transparent";
    static private string _defaultBackgroundHighlight = "#FF9900";

    private RFQ _rfq = new();
    public RFQ RFQ
    {
        get => _rfq;
        set
        {
            if (value != null && _rfq != value)
            {
                Email = value.Requestor?.Id?.Email;
                Name = value.Requestor?.Name;
                Direction = value.Direction?.ToString();
                Notional = value.Notional;
                RollConvention = value.RollConvention?.ToString();
                Product = value.Trade?.Product;
                StartDate = value.StartDate;
                EndDate = value.EndDate;
                Notes = value.Notes;
                OnPropertyChanged();
            }
        }
    }

    private string _emailHighlighted = _defaultBackground;
    public string EmailHighlighted { get => _emailHighlighted; private set => _emailHighlighted = value; }

    public string? Email
    {
        get => _rfq.Requestor?.Id?.Email;
        set
        {
            if (_rfq.Requestor?.Id?.Email != value)
            {
                UpdateProperty(ref _emailHighlighted, _defaultBackgroundHighlight, nameof(EmailHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _emailHighlighted, _defaultBackground, nameof(EmailHighlighted));
                });

                EnsureRequestor();
                _rfq.Requestor.Id.Email = value;
                OnPropertyChanged();
            }
        }
    }

    private string _nameHighlighted = _defaultBackground;
    public string NameHighlighted { get => _nameHighlighted; private set => _nameHighlighted = value; }
    public string? Name
    {
        get => _rfq.Requestor?.Name;
        set
        {
            if (_rfq.Requestor?.Name != value)
            {
                UpdateProperty(ref _nameHighlighted, _defaultBackgroundHighlight, nameof(NameHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _nameHighlighted, _defaultBackground, nameof(NameHighlighted));
                });

                EnsureRequestor();
                _rfq.Requestor.Name = value;
                OnPropertyChanged();
            }
        }
    }

    private string _directionHighlighted = _defaultBackground;
    public string DirectionHighlighted { get => _directionHighlighted; private set => _directionHighlighted = value; }
    public string? Direction
    {
        get => _rfq.Direction?.ToString();
        set
        {
            if (value == null && _rfq.Direction == null)
            {
                return;
            }

            Enum.TryParse(value, out Direction enumVal);
            if (_rfq.Direction != enumVal)
            {
                UpdateProperty(ref _directionHighlighted, _defaultBackgroundHighlight, nameof(DirectionHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _directionHighlighted, _defaultBackground, nameof(DirectionHighlighted));
                });

                _rfq.Direction = enumVal;
                OnPropertyChanged();
            }
        }
    }

    private string _rollConventionHighlighted = _defaultBackground;
    public string RollConventionHighlighted { get => _rollConventionHighlighted; private set => _rollConventionHighlighted = value; }
    public string? RollConvention
    {
        get => _rfq.RollConvention?.ToString();
        set
        {
            if (value == null && _rfq.RollConvention == null)
            {
                return;
            }

            Enum.TryParse(value, out RollConvention enumVal);
            if (_rfq.RollConvention != enumVal)
            {
                UpdateProperty(ref _rollConventionHighlighted, _defaultBackgroundHighlight, nameof(RollConventionHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _rollConventionHighlighted, _defaultBackground, nameof(RollConventionHighlighted));
                });

                _rfq.RollConvention = enumVal;
                OnPropertyChanged();
            }
        }
    }

    private string _productHighlighted = _defaultBackground;
    public string ProductHighlighted { get => _productHighlighted; private set => _productHighlighted = value; }
    public string? Product
    {
        get => _rfq.Trade?.Product;
        set
        {
            if (_rfq.Trade?.Product != value)
            {
                UpdateProperty(ref _productHighlighted, _defaultBackgroundHighlight, nameof(ProductHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _productHighlighted, _defaultBackground, nameof(ProductHighlighted));
                });

                EnsureTrade();
                _rfq.Trade.Product = value;
                OnPropertyChanged();
            }
        }
    }

    private string _notesHighlighted = _defaultBackground;
    public string NotesHighlighted { get => _notesHighlighted; private set => _notesHighlighted = value; }
    public string? Notes
    {
        get => _rfq.Notes;
        set
        {
            if (_rfq.Notes != value)
            {
                UpdateProperty(ref _notesHighlighted, _defaultBackgroundHighlight, nameof(NotesHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _notesHighlighted, _defaultBackground, nameof(NotesHighlighted));
                });

                _rfq.Notes = value;
                OnPropertyChanged();
            }
        }
    }

    private string _startDateHighlighted = _defaultBackground;
    public string StartDateHighlighted { get => _startDateHighlighted; private set => _startDateHighlighted = value; }
    public DateTime? StartDate
    {
        get => _rfq.StartDate;
        set
        {
            if (_rfq.StartDate != value)
            {
                UpdateProperty(ref _startDateHighlighted, _defaultBackgroundHighlight, nameof(StartDateHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _startDateHighlighted, _defaultBackground, nameof(StartDateHighlighted));
                });

                _rfq.StartDate = value;
                OnPropertyChanged();
            }
        }
    }

    private string _endDateHighlighted = _defaultBackground;
    public string EndDateHighlighted { get => _endDateHighlighted; private set => _endDateHighlighted = value; }
    public DateTime? EndDate
    {
        get => _rfq.EndDate;
        set
        {
            if (_rfq.EndDate != value)
            {
                UpdateProperty(ref _endDateHighlighted, _defaultBackgroundHighlight, nameof(EndDateHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _endDateHighlighted, _defaultBackground, nameof(EndDateHighlighted));
                });

                _rfq.EndDate = value;
                OnPropertyChanged();
            }
        }
    }

    private string _notionalHighlighted = _defaultBackground;
    public string NotionalHighlighted { get => _notionalHighlighted; private set => _notionalHighlighted = value; }
    public int? Notional
    {
        get => _rfq.Notional;
        set
        {
            if (_rfq.Notional != value)
            {
                UpdateProperty(ref _notionalHighlighted, _defaultBackgroundHighlight, nameof(NotionalHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _notionalHighlighted, _defaultBackground, nameof(NotionalHighlighted));
                });

                _rfq.Notional = value;
                OnPropertyChanged();
            }
        }

    }

    private void EnsureTrade()
    {
        _rfq.Trade ??= new();
    }

    private void EnsureRequestor()
    {
        _rfq.Requestor ??= new Contact();
        _rfq.Requestor.Id ??= new Identification();
    }
}
