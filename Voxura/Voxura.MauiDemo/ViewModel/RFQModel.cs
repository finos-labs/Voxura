using Voxura.MauiDemo.Model;

namespace Voxura.MauiDemo.ViewModel;

class RFQModel : BaseViewModel
{
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

    private string _emailHighlighted = "White";
    public string EmailHighlighted { get => _emailHighlighted; private set => _emailHighlighted = value; }

    public string? Email
    {
        get => _rfq.Requestor?.Id?.Email;
        set
        {
            if (_rfq.Requestor?.Id?.Email != value)
            {
                UpdateProperty(ref _emailHighlighted, "Yellow", nameof(EmailHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _emailHighlighted, "White", nameof(EmailHighlighted));
                });

                PrepareRequestorIfNeeded();
                _rfq.Requestor.Id.Email = value;
                OnPropertyChanged();
            }
        }
    }

    private string _nameHighlighted = "White";
    public string NameHighlighted { get => _nameHighlighted; private set => _nameHighlighted = value; }
    public string? Name
    {
        get => _rfq.Requestor?.Name;
        set
        {
            if (_rfq.Requestor?.Name != value)
            {
                UpdateProperty(ref _nameHighlighted, "Yellow", nameof(NameHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _nameHighlighted, "White", nameof(NameHighlighted));
                });

                PrepareRequestorIfNeeded();
                _rfq.Requestor.Name = value;
                OnPropertyChanged();
            }
        }
    }

    private string _directionHighlighted = "White";
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
                UpdateProperty(ref _directionHighlighted, "Yellow", nameof(DirectionHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _directionHighlighted, "White", nameof(DirectionHighlighted));
                });

                _rfq.Direction = enumVal;
                OnPropertyChanged();
            }
        }
    }

    private string _rollConventionHighlighted = "White";
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
                UpdateProperty(ref _rollConventionHighlighted, "Yellow", nameof(RollConventionHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _rollConventionHighlighted, "White", nameof(RollConventionHighlighted));
                });

                _rfq.RollConvention = enumVal;
                OnPropertyChanged();
            }
        }
    }

    private string _productHighlighted = "White";
    public string ProductHighlighted { get => _productHighlighted; private set => _productHighlighted = value; }
    public string? Product
    {
        get => _rfq.Trade?.Product;
        set
        {
            if (_rfq.Trade?.Product != value)
            {
                UpdateProperty(ref _productHighlighted, "Yellow", nameof(ProductHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _productHighlighted, "White", nameof(ProductHighlighted));
                });

                PrepareTradeIfNeeded();
                _rfq.Trade.Product = value;
                OnPropertyChanged();
            }
        }
    }

    private string _notesHighlighted = "White";
    public string NotesHighlighted { get => _notesHighlighted; private set => _notesHighlighted = value; }
    public string? Notes
    {
        get => _rfq.Notes;
        set
        {
            if (_rfq.Notes != value)
            {
                UpdateProperty(ref _notesHighlighted, "Yellow", nameof(NotesHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _notesHighlighted, "White", nameof(NotesHighlighted));
                });

                _rfq.Notes = value;
                OnPropertyChanged();
            }
        }
    }

    private string _startDateHighlighted = "White";
    public string StartDateHighlighted { get => _startDateHighlighted; private set => _startDateHighlighted = value; }
    public DateTime? StartDate
    {
        get => _rfq.StartDate;
        set
        {
            if (_rfq.StartDate != value)
            {
                UpdateProperty(ref _startDateHighlighted, "Yellow", nameof(StartDateHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _startDateHighlighted, "White", nameof(StartDateHighlighted));
                });

                _rfq.StartDate = value;
                OnPropertyChanged();
            }
        }
    }

    private string _endDateHighlighted = "White";
    public string EndDateHighlighted { get => _endDateHighlighted; private set => _endDateHighlighted = value; }
    public DateTime? EndDate
    {
        get => _rfq.EndDate;
        set
        {
            if (_rfq.EndDate != value)
            {
                UpdateProperty(ref _endDateHighlighted, "Yellow", nameof(EndDateHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _endDateHighlighted, "White", nameof(EndDateHighlighted));
                });

                _rfq.EndDate = value;
                OnPropertyChanged();
            }
        }
    }

    private string _notionalHighlighted = "White";
    public string NotionalHighlighted { get => _notionalHighlighted; private set => _notionalHighlighted = value; }
    public int? Notional
    {
        get => _rfq.Notional;
        set
        {
            if (_rfq.Notional != value)
            {
                UpdateProperty(ref _notionalHighlighted, "Yellow", nameof(NotionalHighlighted));
                Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                {
                    UpdateProperty(ref _notionalHighlighted, "White", nameof(NotionalHighlighted));
                });

                _rfq.Notional = value;
                OnPropertyChanged();
            }
        }

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
}
