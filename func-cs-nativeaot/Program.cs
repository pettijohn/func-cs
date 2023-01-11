using System.IO;
using System.Net;

public class Program
{
    public static void Main()
    {
        var listenPort = Environment.GetEnvironmentVariable("FUNCTIONS_CUSTOMHANDLER_PORT");
        if(String.IsNullOrEmpty(listenPort)) listenPort = "8080";
        
        Console.WriteLine($"Port: {listenPort}");

        // Create a listener.
        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add($"http://*:{listenPort}/");

            listener.Start();
            Console.WriteLine($"Listening at {listener.Prefixes.FirstOrDefault()}");
            // // Note: The GetContext method blocks while waiting for a request.
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            // // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // // Construct a response.
            response.StatusCode = 200;
            string responseString = shared_logic.ClassThatDoesSomeWork.DoTheWork();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            using (var output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Flush();
                output.Close();
            }
            response.Close();
            listener.Stop();
        }

    }
}