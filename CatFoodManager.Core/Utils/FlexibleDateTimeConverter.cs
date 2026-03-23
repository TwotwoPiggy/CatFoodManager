using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CatFoodManager.Core.Utils;

public class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd",
        "yyyy/MM/dd",
        "yyyy.MM.dd",
        "yyyy年M月d日",
        "yyyy年M月",
        "yyyy-M-d",
        "yyyy/M/d",
        "yyyy.M.d",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyyMMdd"
    ];

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var dateString = reader.GetString();
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return null;
        }

        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        foreach (var format in DateFormats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
