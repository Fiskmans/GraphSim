using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim.Data
{
    public class Rect2IJsonConverter : JsonConverter<Rect2I>
    {
        public override Rect2I Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Not array: " + reader.TokenStartIndex);

            int x;
            int y;
            int w;
            int h;

            if (!reader.Read())
                throw new JsonException("eof");
            if (!reader.TryGetInt32(out x))
                throw new JsonException("not int" + reader.TokenStartIndex);

            if (!reader.Read())
                throw new JsonException("eof");
            if (!reader.TryGetInt32(out y))
                throw new JsonException("not int" + reader.TokenStartIndex);

            if (!reader.Read())
                throw new JsonException("eof");
            if (!reader.TryGetInt32(out w))
                throw new JsonException("not int" + reader.TokenStartIndex);

            if (!reader.Read())
                throw new JsonException("eof");
            if (!reader.TryGetInt32(out h))
                throw new JsonException("not int" + reader.TokenStartIndex);

            if (!reader.Read())
                throw new JsonException("eof");

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException("Not array" + reader.TokenStartIndex);

            return new Rect2I(x, y, w, h);
        }

        public override void Write(Utf8JsonWriter writer, Rect2I value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Position.X);
            writer.WriteNumberValue(value.Position.Y);
            writer.WriteNumberValue(value.Size.X);
            writer.WriteNumberValue(value.Size.Y);
            writer.WriteEndArray();
        }
    }
}
