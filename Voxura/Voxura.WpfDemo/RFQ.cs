using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voxura.WpfDemo;

public class EmailId
{
    [EmailAddress]
    [JsonPropertyName("Email")]
    public string? Email { get; set; }
}

public class Contact
{
    [JsonPropertyName("Id")]
    public EmailId? Id { get; set; } 

    [JsonPropertyName("Name")]
    public string? Name { get; set; } 
}

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

public enum Direction
{
    Buy,
    Sell
}

public class Trade
{
    [JsonPropertyName("Product")]
    public string? Product { get; set; }
}

public enum RollConvention
{
    Following,
    ModifiedFollowing,
    Preceding
}

// Custom converter for date serialization/deserialization, unchanged from previous example
public class JsonDateConverter : JsonConverter<DateTime?>
{
    private const string DateFormat = "yyyy-MM-dd"; // Specify the strict format here

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

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString(DateFormat));
    }
}
