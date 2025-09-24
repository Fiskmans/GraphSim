using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GraphSim.Data
{
    internal class EnumJsonConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return FromString(reader.GetString());
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return FromString(reader.GetString());
        }

        public T FromString(string val)
        {
            if (val == null)
                throw new JsonException($"{typeof(T).Name} is not a string");

            object res;

            if (!Enum.TryParse(typeof(T), val, out res))
                throw new JsonException($"{val} is not in enum {typeof(T).Name}");

            return (T)res;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.ToString());
        }
    }
}
