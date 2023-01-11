# Azure Functions Exploration

This repository explores the performance of Azure Functions and their various hosting models, [In-Process .NET6](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library), [Isolated .NET7](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide), and [Custom](https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-other#create-and-build-your-function) using .NET 7.0 Native AOT. 

## Helpful commands
```
# Build & run two frameworks 
dotnet build && dotnet run -f net6.0 && dotnet run -f net7.0

# Publish Native AOT
dotnet publish -c Release -r linux-x64

# Start Azure Function 
func start 
```