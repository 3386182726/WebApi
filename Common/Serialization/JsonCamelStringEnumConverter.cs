using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Serialization
{
    public class JsonCamelStringEnumConverter : JsonStringEnumConverter
    {
        public JsonCamelStringEnumConverter()
            : base(JsonNamingPolicy.CamelCase) { }

        public JsonCamelStringEnumConverter(
            JsonNamingPolicy? namingPolicy = null,
            bool allowIntegerValues = true
        )
            : base(namingPolicy, allowIntegerValues) { }
    }
}
