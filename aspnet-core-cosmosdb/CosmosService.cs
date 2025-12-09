namespace aspnet_core_cosmosdb;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

/// <summary>
/// Represents Cosmos DB service.
/// </summary>
public class CosmosService(IConfiguration configuration) : ICosmosService
{
    /// <summary>
    /// Database name.
    /// </summary>
    private const string DatabaseName = "ENTER_YOUR_COSMOS_DB_NAME_HERE";

    /// <summary>
    /// Cosmos client.
    /// </summary>
    private readonly CosmosClient _cosmosClient = new(
        configuration["ENTER_YOUR_COSMOS_DB_ENDPOINT_HERE"],
        configuration["ENTER_YOUR_COSMOS_DB_KEY_HERE"],
        new()
        {
            PortReuseMode = PortReuseMode.PrivatePortPool,
            SerializerOptions = new() { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }
        });

    /// <summary>
    /// Gets container.
    /// </summary>
    /// <param name="containerName">Container name</param>
    private Container GetContainer(string containerName) => _cosmosClient.GetContainer(DatabaseName, containerName);

    /// <summary>
    /// Validate params.
    /// </summary>
    /// <param name="args">Params</param>
    private static void ValidateParams(params string[] args)
    {
        if (args.Any(string.IsNullOrWhiteSpace))
            throw new InvalidOperationException("Container name, Document ID or Query string cannot be null or empty.");
    }

    /// <summary>
    /// Reads selected documents contents.
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="documentId">Document ID</param>
    public async Task<T?> GetDocumentAsync<T>(string containerName, string documentId)
    {
        ValidateParams(containerName, documentId);

        try
        {
            Container? container = GetContainer(containerName);

            ItemResponse<T> response = await container.ReadItemAsync<T>(documentId, new(documentId));

            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }

    /// <summary>
    /// Creates a document.
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="item">Document</param>
    public async Task<T> CreateDocumentAsync<T>(string containerName, T item)
    {
        ValidateParams(containerName);

        Container? container = GetContainer(containerName);

        ItemResponse<T> response = await container.CreateItemAsync(item);

        return response.Resource;
    }

    /// <summary>
    /// Returns all documents in selected collection.
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="queryString">Document query</param>
    public async Task<IEnumerable<T>?> ListDocumentsAsync<T>(string containerName, string queryString)
    {
        ValidateParams(containerName, queryString);

        Container? container = GetContainer(containerName);

        FeedIterator<T> query = container.GetItemQueryIterator<T>(new QueryDefinition(queryString));

        List<T> results = [];

        while (query.HasMoreResults)
        {
            FeedResponse<T> response = await query.ReadNextAsync();

            results.AddRange(response);
        }

        return results;
    }

    /// <summary>
    /// Replaces a document.
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="documentId">Document ID</param>
    /// <param name="item">Document</param>
    public async Task<T?> ReplaceDocumentAsync<T>(string containerName, string documentId, T item)
    {
        ValidateParams(containerName);

        Container? container = GetContainer(containerName);

        ItemResponse<T> response = await container.UpsertItemAsync(item, new(documentId));

        return response.Resource;
    }

    /// <summary>
    /// Deletes selected document.
    /// </summary>
    /// <param name="containerName">Container name</param>
    /// <param name="documentId">Document ID</param>
    public async Task<bool> DeleteDocumentAsync<T>(string containerName, string documentId)
    {
        ValidateParams(containerName, documentId);

        try
        {
            Container? container = GetContainer(containerName);

            await container.DeleteItemAsync<T>(documentId, new(documentId));

            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}