# Azure Functions Performance Exploration

This repository explores the performance of Azure Functions and their various hosting models, [In-Process .NET6](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), [Isolated .NET7](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide), and [Custom](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-other#create-and-build-your-function) using .NET 7.0 Native AOT. 

Microsoft [describes](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/) .NET 7.0 Native AOT: "The benefit of native AOT is most significant for workloads with a high number of deployed instances, such as cloud infrastructure and hyper-scale services."  They [also say](https://visualstudiomagazine.com/articles/2022/04/15/net-7-preview-3.aspx): "native AOT promises to help build faster, lighter apps, while explaining what it is for those not familiar, noting that its main advantages affect: Startup time, Memory usage, Access to restricted platforms (where no just-in time (JIT) compilation is allowed), Smaller size on disk." [Also](https://devblogs.microsoft.com/dotnet/announcing-dotnet-7-preview-3/): "Native AOT is best suited for environments where startup time matters the most." Compelling, especially for Functions-as-a-Service!

But! Azure Functions only support in-process for long term support versions of .NET (6.0); you can only run NET 7.0 in isolated processes. Both support ReadyToRun (a form of AOT compilation), and it looks like they both use reflection to identify Functions (relatively slow at startup to discover them all). There is no out-of-the-box way to run .NET 7 Native AOT as Azure Functions. This repository explores that, spinning up an HTTP Listener in C#, and compares performance of the three approaches. 

The `shared-logic` project targets net6.0 and net7.0, and uses source generation for JSON serialization in net7.0 only. The `ClassThatDoesSomeWork` creates a (small) object, serializes it to JSON, encrypts it with AES, converts to Base64 and returns the string. It's meant to represent a typical cloud function workload, such as validating a JWT & serializing data to JSON. Since we're interested in Azure Function performance, especially cold & warm start, there is no IO to e.g. a database, only the pure compute workload of the Function. 

## Helpful commands
```
# Build & run two frameworks 
dotnet build && dotnet run -f net6.0 && dotnet run -f net7.0

# Publish Native AOT
dotnet publish -c Release -r linux-x64

# Start Azure Function 
func start 
```

## References

* [Working with Azure Functions CLI](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=azure-cli%2Cin-process)
* [Installing .NET 6 and 7 side by side on Ubuntu 22.04 LTS](https://github.com/dotnet/core/issues/7699) - You need to install both versions from PMC and set the Priority to `1001` for `Packages: *`. Follow Clean Machine Scenario 2 with additional steps: first, `sudo apt remove dotnet* && sudo apt remove aspnetcore*`, finally, `sudo apt install dotnet-sdk-6.0 dotnet-sdk-7.0`.
* [Azure Functions Source Repositories](https://learn.microsoft.com/en-us/azure/azure-functions/functions-reference?tabs=blob#repositories)