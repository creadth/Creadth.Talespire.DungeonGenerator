using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Creadth.Talespire.DungeonGenerator.Framework
{
    public class Vector3Serializer : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            var x = reader.GetSingle();
            reader.Read();
            reader.Read();
            var y = reader.GetSingle();
            reader.Read();
            reader.Read();
            var z = reader.GetSingle();
            var res = new Vector3(x, y, z);
            reader.Read();
            return res;
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(JsonEncodedText.Encode("x"), value.X);
            writer.WriteNumber(JsonEncodedText.Encode("y"), value.Y);
            writer.WriteNumber(JsonEncodedText.Encode("z"), value.Z);
            writer.WriteEndObject();
        }
    }
}
