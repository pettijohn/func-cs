using System;
using System.Diagnostics;
using System.Net.Http;

var endpoints = new (string, string)[]
    {
        ("func-cs-inproc", "http://localhost:7071/api/InProcExample"),
        ("func-cs-isolated", "http://localhost:7072/api/IsolatedExample"),
        ("func-cs-native", "http://localhost:7073/api/HttpTrigger1")
    };

// var commandArgs = "start --enable-json-output --json-output-file ../test-harness/logs/{0}.json";

// var startInfo = new ProcessStartInfo("func") { 
//     WorkingDirectory = "/mnt/c/Users/Travis/Code/func-cs/func-cs-inproc",
//     Arguments = String.Format(commandArgs, "func-cs-inproc")
//  };
// var process = Process.Start(startInfo);
// if (process == null) throw new Exception("Func process did not start");
// Thread.Sleep(TimeSpan.FromSeconds(10));

var client = new HttpClient();
var sw = Stopwatch.StartNew();
long nanosecPerTick = (1000L*1000L*1000L) / Stopwatch.Frequency;


for (int i = 0; i < 10000; i++)
{
    foreach (var endpoint in endpoints)
    {
        sw.Restart();
        string _ = await client.GetStringAsync(endpoint.Item2);
        var elapsed = sw.ElapsedTicks * nanosecPerTick;
        Console.WriteLine($"{endpoint.Item1},{elapsed}");    
    }
    
}

