using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CatFoodManager.Core.Utils;

/// <summary>
/// 灵活的日期时间转换器，支持多种日期格式的解析。
/// Flexible datetime converter, supporting parsing of multiple date formats.
/// </summary>
public class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    /// <summary>
    /// 支持的日期格式数组。
    /// Array of supported date formats.
    /// </summary>
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

    /// <summary>
    /// 读取JSON并转换为可空日期时间。
    /// Reads JSON and converts to nullable datetime.
    /// </summary>
    /// <param name="reader">JSON读取器 / JSON reader</param>
    /// <param name="typeToConvert">要转换的类型 / Type to convert</param>
    /// <param name="options">JSON序列化选项 / JSON serializer options</param>
    /// <returns>解析后的可空日期时间 / Parsed nullable datetime</returns>
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

    /// <summary>
    /// 将可空日期时间写入JSON。
    /// Writes nullable datetime to JSON.
    /// </summary>
    /// <param name="writer">JSON写入器 / JSON writer</param>
    /// <param name="value">要写入的可空日期时间 / Nullable datetime to write</param>
    /// <param name="options">JSON序列化选项 / JSON serializer options</param>
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
