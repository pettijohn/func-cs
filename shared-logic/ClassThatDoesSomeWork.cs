using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace shared_logic;

public class Dimensions
{
    [JsonConstructorAttribute]
    public Dimensions() { }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Depth { get; set; }
}

#if NET7_0
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Dimensions))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
#endif

public static class ClassThatDoesSomeWork
{
    public static string DoTheWork()
    {
        // Create an object
        var dims = new Dimensions() { Height = 4.5, Width = 6.7, Depth = 8.9 };

        // Serialize it to JSON
#if NET7_0
        var json_dims = JsonSerializer.Serialize<Dimensions>(dims, new JsonSerializerOptions() { TypeInfoResolver = SourceGenerationContext.Default });
#endif
#if NET6_0
        var json_dims = JsonSerializer.Serialize<Dimensions>(dims);
#endif
        // Generate an AES Key

        // Encrypt the JSON

        // Encode as Base64

        // Return string

#if NET6_0
        return "Hello from .NET 6.0" + json_dims;
#endif

#if NET7_0
        return "Hello from .NET 7.0" + json_dims;
#endif
    }
}
