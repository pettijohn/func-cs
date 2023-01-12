using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;

public class Program
{
    public static void Main()
    {
        try
        {
            var listenPort = Environment.GetEnvironmentVariable("FUNCTIONS_CUSTOMHANDLER_PORT");
            if (String.IsNullOrEmpty(listenPort)) listenPort = "8080";

            //Debug.WriteLine($"Port: {listenPort}");

            // Create a listener.
            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add($"http://*:{listenPort}/");

                listener.Start();
                // Graceful shutdown?
                Console.CancelKeyPress += (_, _) => { listener.Abort(); };
                AppDomain.CurrentDomain.ProcessExit += (_, _) => { listener.Abort(); };

                Debug.WriteLine($"➡️ Internal listener at {listener.Prefixes.FirstOrDefault()}");

                while (listener.IsListening)
                {
                    // // Note: The GetContext method blocks while waiting for a request.
                    HttpListenerContext context = listener.GetContext();
                    Debug.WriteLine("➡️ Request received");
                    HttpListenerRequest request = context.Request;
                    Debug.WriteLine($"➡️ Raw URL: {request.RawUrl}");
                    Debug.WriteLine($"➡️ Method: {request.HttpMethod}");

                    // // Obtain a response object.
                    HttpListenerResponse response = context.Response;
                    // // Construct a response.
                    response.StatusCode = 200;
                    // Debug.WriteLine("➡️ Status code set");
                    string responseString = shared_logic.ClassThatDoesSomeWork.DoTheWork();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    using (var output = response.OutputStream)
                    {
                        output.Write(buffer, 0, buffer.Length);
                        Debug.WriteLine("➡️ Response written");

                        // You must close the output stream.
                        output.Flush();
                        output.Close();
                    }
                    response.Close();
                }
                listener.Stop();
            }
        }
        finally
        {
            Environment.Exit(0);
        }
    }
}