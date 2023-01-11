using System.IO;
using System.Net;

public class Program
{
    public static void Main()
    {
        var listenPort = Environment.GetEnvironmentVariable("FUNCTIONS_CUSTOMHANDLER_PORT");
        if(String.IsNullOrEmpty(listenPort)) listenPort = "8080";
        
        //Console.WriteLine($"Port: {listenPort}");

        // Create a listener.
        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add($"http://*:{listenPort}/");

            listener.Start();

            Console.WriteLine($"➡️ Internal listener at {listener.Prefixes.FirstOrDefault()}");

            while (listener.IsListening)
            {

                // // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                Console.WriteLine("➡️ Request received");
                HttpListenerRequest request = context.Request;
                using (var s = request.InputStream)
                {
                    int b = -1;
                    do
                    {
                        b = s.ReadByte();

                    } while (b != -1);
                }
                // // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // // Construct a response.
                response.StatusCode = 200;
                Console.WriteLine("➡️ Status code set");
                string responseString = shared_logic.ClassThatDoesSomeWork.DoTheWork();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                using (var output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("➡️ Response written");

                    // You must close the output stream.
                    output.Flush();
                    output.Close();
                }
                response.Close();
            }
            listener.Stop();
        }

    }
}