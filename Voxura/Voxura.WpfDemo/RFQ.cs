using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voxura.WpfDemo;

/// <summary>
/// Represents an email based ID
/// </summary>
public class EmailId
{
    [EmailAddress]
    [JsonPropertyName("Email")]
    public string? Email { get; set; }
}

/// <summary>
/// Represents a contact
/// </summary>
public class Contact
{
    [JsonPropertyName("Id")]
    public EmailId? Id { get; set; } 

    [JsonPropertyName("Name")]
    public string? Name { get; set; } 
}

/// <summary>
/// Represents a request for quote (RFQ) object
/// </summary>
public class RFQ
{
    [JsonPropertyName("Requestor")]
    public Contact? Requestor { get; set; } 

    [JsonPropertyName("Direction")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Direction? Direction { get; set; }

    [JsonPropertyName("Notional")]
    public int? Notional { get; set; } 

    [JsonPropertyName("StartDate")]
    [JsonConverter(typeof(JsonDateConverter))]
    public DateTime? StartDate { get; set; } 

    [JsonPropertyName("EndDate")]
    [JsonConverter(typeof(JsonDateConverter))]
    public DateTime? EndDate { get; set; } 

    [JsonPropertyName("RollConvention")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RollConvention? RollConvention { get; set; } 

    [JsonPropertyName("Trade")]
    public Trade? Trade { get; set; } 

    [JsonPropertyName("Notes")]
    public string? Notes { get; set; }
}

/// <summary>
/// Enum for the direction of the trade (buy or sell)
/// </summary>
public enum Direction
{
    Buy,
    Sell
}

/// <summary>
/// Represents a trade object
/// </summary>
public class Trade
{
    [JsonPropertyName("Product")]
    public string? Product { get; set; }
}

/// <summary>
/// Enum for the possible roll convention values
/// </summary>
public enum RollConvention
{
    Following,
    ModifiedFollowing,
    Preceding
}

/// <summary>
/// Json converter for date objects
/// </summary>
public class JsonDateConverter : JsonConverter<DateTime?>
{
    private const string DateFormat = "yyyy-MM-dd"; // Specify the strict format here

    /// <inheritdoc cref="JsonConverter{T}.Read"/>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (dateString == null) return null;
        if (DateTime.TryParseExact(dateString, DateFormat, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }
        throw new JsonException("Date format is not valid.");
    }

    /// <inheritdoc cref="JsonConverter{T}.Write"/>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString(DateFormat));
    }
}
