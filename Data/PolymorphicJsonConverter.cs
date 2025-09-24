using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Godot.HttpRequest;

namespace GraphSim.Data
{
    public class PolymorphicJsonConverter<T> : JsonConverter<T> where T : TypedJsonObject
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Utf8JsonReader clone = reader;

            T b = JsonSerializer.Deserialize<T>(ref clone);

            Type type = Assembly.GetAssembly(typeof(T)).GetType(b.Type);

            if (type == null)
                throw new JsonException($"Type {b.Type} does not exist in the assembly");

            if (!type.IsSubclassOf(typeof(T)))
                throw new JsonException($"{b.Type} does not inherit from {typeof(T).Name}");

            return (T)JsonSerializer.Deserialize(ref reader, type, options);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            //TODO if i want to edit the data from inside the game
            throw new NotImplementedException();
        }
    }
}
