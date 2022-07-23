using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TapeRecordWizard.Models;

namespace TapeRecordWizard.Json
{
    public class SongConverter : JsonConverter<Song>
    {
        public override Song Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("No token start object");
            }
            Song result = null;
            string fullFilePath = string.Empty;
            string side = string.Empty;
            int orderNo = 0;
            bool isVirtual = false;
            TimeSpan duration = new TimeSpan();
            while(reader.Read())
            {
                if(reader.TokenType == JsonTokenType.EndObject)
                {
                    if(isVirtual)
                    {
                        return new Song(fullFilePath, duration) { Side = side, OrderNo = orderNo };
                    }
                    else
                    {
                        return new Song() { FullFilePath = fullFilePath, Side = side, OrderNo = orderNo };
                    }
                }
                string? propertyName = reader.GetString();
                switch(propertyName)
                {
                    case nameof(Song.FullFilePath):
                        {
                            reader.Read();
                            fullFilePath = reader.GetString();
                        }
                        break;
                    case nameof(Song.Side):
                        {
                            reader.Read();
                            side = reader.GetString();
                        }
                    break;
                    case nameof(Song.OrderNo):
                        {
                            reader.Read();
                            orderNo = reader.GetInt16();
                        }
                        break;
                    case nameof(Song.IsVirtual):
                        {
                            reader.Read();
                            isVirtual = reader.GetBoolean();
                        }
                        break;
                    case nameof(Song.Duration):
                        {
                            reader.Read();
                            duration = new Json.TimeSpanConverter().Read(ref reader, typeof(TimeSpan), options);
                        }
                        break;
                }

            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Song value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(Song.FullFilePath), value.FullFilePath);
            writer.WriteString(nameof(Song.Side), value.Side);
            writer.WriteNumber(nameof(Song.OrderNo), value.OrderNo);
            writer.WriteBoolean(nameof(Song.IsVirtual), value.IsVirtual);
            writer.WritePropertyName(nameof(Song.Duration));
            new Json.TimeSpanConverter().Write(writer, value.Duration, options);
            writer.WriteEndObject();
        }
    }
}
