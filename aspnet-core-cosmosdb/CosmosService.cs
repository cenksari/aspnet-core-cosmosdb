namespace aspnet_core_cosmosdb;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

/// <summary>
/// Cosmos DB service.
/// </summary>
public class CosmosService : ICosmosService
{
	private readonly CosmosClient _cosmosClient;

	private const string DatabaseId = "ENTER_YOUR_COSMOS_DB_NAME_HERE";

	private readonly CosmosClientOptions options = new()
	{
		PortReuseMode = PortReuseMode.PrivatePortPool,
		SerializerOptions = new()
		{
			PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
		}
	};

	public CosmosService(IConfiguration configuration) => _cosmosClient = new(configuration["CosmosDbEndpoint"], configuration["CosmosDbKey"], options);

	/// <summary>
	/// Reads selected documents contents.
	/// </summary>
	/// <param name="containerName">Container name</param>
	/// <param name="documentId">Document ID</param>
	public async Task<T?> GetDocumentAsync<T>(string containerName, string documentId)
	{
		Container? container = _cosmosClient.GetContainer(DatabaseId, containerName);

		try
		{
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
		Container container = _cosmosClient.GetContainer(DatabaseId, containerName);

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
		Container container = _cosmosClient.GetContainer(DatabaseId, containerName);

		FeedIterator<T> query = container.GetItemQueryIterator<T>(new QueryDefinition(queryString));

		List<T> results = [];

		while (query.HasMoreResults)
		{
			FeedResponse<T> response = await query.ReadNextAsync();

			foreach (T item in response) { results.Add(item); }
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
		if (!string.IsNullOrEmpty(documentId))
		{
			Container container = _cosmosClient.GetContainer(DatabaseId, containerName);

			ItemResponse<T> response = await container.UpsertItemAsync(item, new(documentId));

			return response.Resource;
		}

		return default;
	}

	/// <summary>
	/// Deletes selected document.
	/// </summary>
	/// <param name="containerName">Container name</param>
	/// <param name="documentId">Document ID</param>
	public async Task<T?> DeleteDocumentAsync<T>(string containerName, string documentId)
	{
		if (!string.IsNullOrEmpty(documentId))
		{
			Container container = _cosmosClient.GetContainer(DatabaseId, containerName);

			ItemResponse<T> response = await container.DeleteItemAsync<T>(documentId, new(documentId));

			return response.Resource;
		}

		return default;
	}
}