namespace aspnet_core_cosmosdb;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

/// <summary>
/// Cosmos DB service.
/// </summary>
public class CosmosService(IConfiguration configuration) : ICosmosService
{
	private const string DatabaseName = "ENTER_YOUR_COSMOS_DB_NAME_HERE";

	private readonly CosmosClient _cosmosClient = new(
		configuration["CosmosDbEndpoint"],
		configuration["CosmosDbKey"],
		new()
		{
			PortReuseMode = PortReuseMode.PrivatePortPool,
			SerializerOptions = new()
			{
				PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
			}
		});

	/// <summary>
	/// Reads selected documents contents.
	/// </summary>
	/// <param name="containerName">Container name</param>
	/// <param name="documentId">Document ID</param>
	public async Task<T?> GetDocumentAsync<T>(string containerName, string documentId)
	{
		if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(documentId))
			throw new ArgumentException("Container name or Document Id cannot be null or empty.");

		try
		{
			Container? container = _cosmosClient.GetContainer(DatabaseName, containerName);

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
		if (string.IsNullOrEmpty(containerName))
			throw new ArgumentException("Container name cannot be null or empty.");

		try
		{
			Container? container = _cosmosClient.GetContainer(DatabaseName, containerName);

			ItemResponse<T> response = await container.CreateItemAsync(item);

			return response.Resource;
		}
		catch
		{
			throw;
		}
	}

	/// <summary>
	/// Returns all documents in selected collection.
	/// </summary>
	/// <param name="containerName">Container name</param>
	/// <param name="queryString">Document query</param>
	public async Task<IEnumerable<T>?> ListDocumentsAsync<T>(string containerName, string queryString)
	{
		if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(queryString))
			throw new ArgumentException("Container name or query string cannot be null or empty.");

		Container? container = _cosmosClient.GetContainer(DatabaseName, containerName);

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
		if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(documentId))
			throw new ArgumentException("Container name or Document Id cannot be null or empty.");

		try
		{
			Container? container = _cosmosClient.GetContainer(DatabaseName, containerName);

			ItemResponse<T> response = await container.UpsertItemAsync(item, new(documentId));

			return response.Resource;
		}
		catch
		{
			throw;
		}
	}

	/// <summary>
	/// Deletes selected document.
	/// </summary>
	/// <param name="containerName">Container name</param>
	/// <param name="documentId">Document ID</param>
	public async Task<T?> DeleteDocumentAsync<T>(string containerName, string documentId)
	{
		if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(documentId))
			throw new ArgumentException("Container name or Document Id cannot be null or empty.");

		try
		{
			Container? container = _cosmosClient.GetContainer(DatabaseName, containerName);

			ItemResponse<T> response = await container.DeleteItemAsync<T>(documentId, new(documentId));

			return response.Resource;
		}
		catch
		{
			throw;
		}
	}
}