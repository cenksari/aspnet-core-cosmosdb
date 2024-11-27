
# ASP.NET Core Cosmos DB Service

**A streamlined and efficient service for interacting with Azure Cosmos DB in ASP.NET Core applications.**

---

## 📋 Features

- **Retrieve Documents**: Fetch specific documents from a Cosmos DB container.
- **Create Documents**: Insert new items into a container.
- **List Documents**: Query and retrieve multiple documents.
- **Update Documents**: Replace existing items with updated data.
- **Delete Documents**: Remove items by their document ID.
- **Configurable**: Easily integrates with your app's configuration system.

---

## 🚀 Getting Started

### 1. Prerequisites
- **.NET 9.0+**
- **Azure Cosmos DB Account**
- **Azure SDK**: Ensure you have the `Microsoft.Azure.Cosmos` NuGet package installed.

### 2. Installation

Add the required NuGet package to your project:
```bash
dotnet add package Microsoft.Azure.Cosmos
```

Include the `CosmosService` class in your project and configure it as a service.

### 3. Configuration

Add the following settings to your `appsettings.json` file:
```json
{
  "CosmosDbEndpoint": "https://your-cosmos-account.documents.azure.com:443/",
  "CosmosDbKey": "your-cosmos-primary-key",
  "CosmosDbName": "your-database-name"
}
```

---

## 🛠️ Usage

### Inject and Use the Service
Register `CosmosService` in your dependency injection container:

```csharp
services.AddSingleton<ICosmosService, CosmosService>();
```

Use it in your controllers or services:

```csharp
public class MyController : ControllerBase
{
    private readonly ICosmosService _cosmosService;

    public MyController(ICosmosService cosmosService)
    {
        _cosmosService = cosmosService;
    }

    [HttpGet("document/{id}")]
    public async Task<IActionResult> GetDocument(string id)
    {
        var document = await _cosmosService.GetDocumentAsync<MyDocument>("my-container", id);
        return document is not null ? Ok(document) : NotFound();
    }
}
```

---

## ✨ Key Methods

### 1. Get a Document
```csharp
await cosmosService.GetDocumentAsync<MyType>("containerName", "documentId");
```

### 2. Create a Document
```csharp
await cosmosService.CreateDocumentAsync("containerName", new MyType { ... });
```

### 3. List Documents
```csharp
await cosmosService.ListDocumentsAsync<MyType>("containerName", "SELECT * FROM c");
```

### 4. Replace a Document
```csharp
await cosmosService.ReplaceDocumentAsync("containerName", "documentId", updatedItem);
```

### 5. Delete a Document
```csharp
await cosmosService.DeleteDocumentAsync<MyType>("containerName", "documentId");
```

---

## 💡 Contributions

Contributions are welcome! Feel free to open an issue or submit a pull request to improve this project.

---

## 📜 License

This project is licensed under the **MIT License**.

---

## 🌟 Acknowledgments

Thanks to the **Azure SDK team** for their amazing libraries and tools!
