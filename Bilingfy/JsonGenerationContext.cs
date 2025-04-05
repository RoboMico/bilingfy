using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bilingfy;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class JsonGenerationContext : JsonSerializerContext
{

}