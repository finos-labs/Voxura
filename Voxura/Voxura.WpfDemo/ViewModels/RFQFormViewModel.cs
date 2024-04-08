using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Voxura.WpfDemo.ViewModels;
    
/// <summary>
/// Represents the view model for the RFQ form.
/// </summary>
public class RFQFormViewModel : ObservableObject
{
    private string? _email;

    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }
    
    private string? _name;

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    private Direction? _direction;

    public Direction? Direction
    {
        get => _direction;
        set => SetProperty(ref _direction, value);
    }
    
    private int? _notional;

    public int? Notional
    {
        get => _notional;
        set => SetProperty(ref _notional, value);
    }

    private DateTime? _startDate;

    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }
    
    private DateTime? _endDate;

    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }
    
    private RollConvention? _rollConvention;

    public RollConvention? RollConvention
    {
        get => _rollConvention;
        set => SetProperty(ref _rollConvention, value);
    }
    
    private string? _product;

    public string? Product
    {
        get => _product;
        set => SetProperty(ref _product, value);
    }
    
    private string? _notes;

    public string? Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }


    public List<Direction> AllDirections { get; } = Enum.GetValues<Direction>().ToList();

    public List<RollConvention> AllRollConventions { get; } = Enum.GetValues<RollConvention>().ToList();


    public void SetFromRFQ(RFQ rfq)
    {
        Email = rfq.Requestor?.Id?.Email;
        Name = rfq.Requestor?.Name;
        Direction = rfq.Direction;
        Notional = rfq.Notional;
        StartDate = rfq.StartDate;
        EndDate = rfq.EndDate;
        RollConvention = rfq.RollConvention;
        Product = rfq.Trade?.Product;
        Notes = rfq.Notes;
    }
}
