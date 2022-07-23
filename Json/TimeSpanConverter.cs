using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TapeRecordWizard.Json
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("No token start object");
            }
            var result = new TimeSpan();

            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            while (reader.Read())
            {
                if(reader.TokenType == JsonTokenType.EndObject)
                {
                    return new TimeSpan(hours, minutes, seconds);
                }

                string? propertyName = reader.GetString();
                if (propertyName == nameof(TimeSpan.Hours))
                {
                    reader.Read();
                    hours = reader.GetInt32();
                    reader.Read();
                    propertyName = reader.GetString();
                    if(propertyName == nameof(TimeSpan.Minutes))
                    {
                        reader.Read();
                        minutes = reader.GetInt32();
                        reader.Read();
                        propertyName = reader.GetString();
                        if (propertyName == nameof(TimeSpan.Seconds))
                        {
                            reader.Read();
                            seconds = reader.GetInt32();
                        }
                    }
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(TimeSpan.Hours), value.Hours);
            writer.WriteNumber(nameof(TimeSpan.Minutes), value.Minutes);
            writer.WriteNumber(nameof(TimeSpan.Seconds), value.Seconds);
            writer.WriteEndObject();
        }
    }
}
