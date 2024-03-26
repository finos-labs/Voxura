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
                Email = value.Requestor?.Id?.Email ?? string.Empty;
                Name = value.Requestor?.Name ?? string.Empty;
                Direction = value.Direction.ToString();
                Notional = value.Notional;
                RollConvention = value.RollConvention.ToString();
                Product = value.Trade?.Product;
                StartDate = value.StartDate;
                EndDate = value.EndDate;
                Notes = value.Notes;
                OnPropertyChanged();
            }
        }
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