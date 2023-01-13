using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Security.Cryptography;

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
        var iterations = 10000;
        string? returnValue = null;
        for (int i = 0; i < iterations; i++)
        {
#if NET6_0
            var net = "6.0"; ;
#endif
#if NET7_0
            var net = "7.0";
#endif
            // Create an object
            var dims = new Dimensions() { Height = 4.5, Width = 6.7, Depth = 8.9 };


            // Serialize it to JSON
#if NET7_0
            var json_dims = JsonSerializer.Serialize<Dimensions>(dims, new JsonSerializerOptions() { TypeInfoResolver = SourceGenerationContext.Default });
#endif
#if NET6_0
            var json_dims = JsonSerializer.Serialize<Dimensions>(dims);
#endif

            // Encrypt the JSON
            byte[] encrypted;
            // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-7.0
            using (Aes myAes = Aes.Create())
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = myAes.Key;
                    aesAlg.IV = myAes.IV;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(json_dims);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
            }
            if (encrypted == null || encrypted.Length == 0) throw new Exception("Encryption failed");

            // Encode as Base64
            var b64 = Convert.ToBase64String(encrypted);

            // Return string
            returnValue = $"From .NET {net}! Here's some JSON: {json_dims}. Here it is encrypted: {b64}.";
        }
        return returnValue ?? "Error";
    }
}
