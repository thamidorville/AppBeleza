namespace BelezaApp.API.Converters;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private const string Formato = "HH:mm";

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeOnly.ParseExact(reader.GetString()!, Formato, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Formato, CultureInfo.InvariantCulture));
    }
}
