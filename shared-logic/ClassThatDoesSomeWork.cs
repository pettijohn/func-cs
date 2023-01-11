namespace shared_logic;




public class Dimensions
{
    public double Height { get; set; }
    public double Width { get; set; }
    public double Depth { get; set; }
}

public static class ClassThatDoesSomeWork
{
    public static string DoTheWork()
    {
#if NET6_0
        return "Hello from .NET 6.0";
#endif

#if NET7_0
        return "Hello from .NET 7.0";
#endif
        // Create an object

        // Serialize it to JSON

        // Generate an AES Key

        // Encrypt the JSON

        // Encode as Base64

        // Return string
    }
}
