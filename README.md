# Azure Functions Performance Exploration

## Or, How to run .NET 7 Native AOT in an Azure Function (and is it a good idea?)

**TL;DR: For C#, .NET 6 In-Process appears to be the best choice** - performance is the best, and it is a a fully-featured, supported out-of-box solution. Native AOT *may* have cold start benefits, but you'll have to roll your own HTTP listener, router, function mapper, etc. 

This repository explores the performance of Azure Functions and their various hosting models, [In-Process .NET6](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), [Isolated .NET7](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide), and [Custom](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-other#create-and-build-your-function) using .NET 7.0 Native AOT. 

Microsoft [describes](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/) .NET 7.0 Native AOT: "The benefit of native AOT is most significant for workloads with a high number of deployed instances, such as cloud infrastructure and hyper-scale services."  They [also say](https://visualstudiomagazine.com/articles/2022/04/15/net-7-preview-3.aspx): "native AOT promises to help build faster, lighter apps, while explaining what it is for those not familiar, noting that its main advantages affect: Startup time, Memory usage, Access to restricted platforms (where no just-in time (JIT) compilation is allowed), Smaller size on disk." [Also](https://devblogs.microsoft.com/dotnet/announcing-dotnet-7-preview-3/): "Native AOT is best suited for environments where startup time matters the most." Compelling, especially for Functions-as-a-Service!

But! Azure Functions only support in-process for long term support versions of .NET (6.0); you can only run NET 7.0 in isolated processes. Both support ReadyToRun (a form of AOT compilation), and it looks like they both use reflection to identify Functions (relatively slow at startup to discover them all). There is no out-of-the-box way to run .NET 7 Native AOT as Azure Functions. This repository explores that, spinning up an HTTP Listener in C#, and compares performance of the three approaches. 

The `shared-logic` project targets net6.0 and net7.0, and uses source generation for JSON serialization in net7.0 only. The `ClassThatDoesSomeWork` creates a (small) object, serializes it to JSON, encrypts it with AES, converts to Base64 and returns the string. It's meant to represent a typical cloud function workload, such as validating a JWT & serializing data to JSON. Since we're interested in Azure Function performance, especially cold & warm start, there is no IO to e.g. a database, only the pure compute workload of the Function. 

## Results

Average of 10,000 iterations each, includes cold & warm. Functions called in round robin (one call to each, then repeat) in case the CPU throttles as the test drags on. Cold start table is fifty invocation of each method (restart the `func` process each time). I repeated these tests and, while the numbers varied from run to run, the pattern held true: inproc was fastest, native was slightly slower, and isolated was a bit slower yet. 

I assume that In-Process wins because, despite Native AOT's performance benefits, In-Process can just pass an object pointer for each function call as opposed to the overhead of out-of-process HTTP over a pipe. Native AOT *may* be a better fit for cases where the compute cost is considerably higher than the cost of out-of-process HTTP. I also didn't measure memory consumption, so Native AOT *may* be less expensive in some cases as well. 

| Method | Average response (ms) 10k runs | StdDev |
|-|-|-|
| func-cs-inproc    | 2.12 | 3.48 |
| func-cs-isolated  | 3.52 | 6.05 |
| func-cs-nativeaot | 2.77 | 2.90 | 

| Cold start | Avg of 50 runs (ms) |
|-|-|
| func-cs-inproc    | 280.39 |
| func-cs-isolated  | 544.15 |
| func-cs-nativeaot | 277.66 |

Caveats: These tests were run with the [local functions tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local) on a Windows laptop under WSL2. Who knows what noise there is in the data or how this would behave on Azure's servers. If your function is most often called cold, you may wish to further explore the potential cold start benefits of Native AOT. 

## Helpful commands
```
# Build & run two frameworks 
dotnet build && dotnet run -f net6.0 && dotnet run -f net7.0

# Publish Native AOT
dotnet publish -c Release -r linux-x64

# Start Azure Function 
func start 
func start --port 7071
```

## References

* [Working with Azure Functions CLI](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=azure-cli%2Cin-process)
* [Installing .NET 6 and 7 side by side on Ubuntu 22.04 LTS](https://github.com/dotnet/core/issues/7699) - You need to install both versions from PMC and set the Priority to `1001` for `Packages: *`. Follow Clean Machine Scenario 2 with additional steps: first, `sudo apt remove dotnet* && sudo apt remove aspnetcore*`, finally, `sudo apt install dotnet-sdk-6.0 dotnet-sdk-7.0`.
* [Azure Functions Source Repositories](https://learn.microsoft.com/en-us/azure/azure-functions/functions-reference?tabs=blob#repositories)
* [Functions Custom Handlers Samples](https://github.com/Azure-Samples/functions-custom-handlers)