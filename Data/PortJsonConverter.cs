using GraphSim.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace GraphSim.Data
{
    public class PortJsonConverter : JsonConverter<Port>
    {
        public override Port Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Not array");

            Port result = new Port();

            if (!reader.Read())
                throw new JsonException("eof");
            int x = reader.GetInt32();
            if (!reader.Read())
                throw new JsonException("eof");
            int y = reader.GetInt32();

            result.Position = new Godot.Vector2I(x,y);

            if (!reader.Read())
                throw new JsonException("eof");
            result.Direction = Enum.Parse<Direction>(reader.GetString());

            if (!reader.Read())
                throw new JsonException("eof");
            result.Type = Enum.Parse<PortType>(reader.GetString());

            if (!reader.Read())
                throw new JsonException("eof");

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException("Not array");

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Port value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Position.X);
            writer.WriteNumberValue(value.Position.Y);
            writer.WriteStringValue(value.Direction.ToString());
            writer.WriteEndArray();
        }
    }
}
