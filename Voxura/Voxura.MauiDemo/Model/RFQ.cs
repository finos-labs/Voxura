
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voxura.MauiDemo.Model;

public enum Direction
{
    Buy,
    Sell
}

public enum RollConvention
{
    Following,
    ModifiedFollowing,
    Preceding
}

public class Identification
{
    public string? Email { get; set; }

}

public class Contact
{
    public Identification? Id { get; set; }

    public string? Name { get; set; }
}


public class Trade
{
    public string? Product { get; set; }
}


public class RFQ
{
    public Contact? Requestor { get; set; }

    public int? Notional { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Direction? Direction { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RollConvention? RollConvention { get; set; }

    public Trade? Trade { get; set; }

    public string? Notes { get; set; }
}
