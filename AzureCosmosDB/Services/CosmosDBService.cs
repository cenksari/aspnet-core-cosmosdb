namespace AzureCosmosDB.Services
{
	using Microsoft.Azure.Cosmos;
	using Microsoft.Extensions.Configuration;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;

	/// <summary>
	/// Cosmos DB service.
	/// </summary>
	public class CosmosDBService : ICosmosDBService
	{
		private readonly CosmosClient CosmosClient;

		private const string DatabaseId = "cosmosdbtest";

		private readonly CosmosClientOptions options = new()
		{
			PortReuseMode = PortReuseMode.PrivatePortPool,
			SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }
		};

		public CosmosDBService(IConfiguration configuration) => CosmosClient = new(configuration["CosmosDbEndpoint"], configuration["CosmosDbKey"], options);

		/// <summary>
		/// Return all documents in selected collection.
		/// </summary>
		/// <param name="containerName">Container name</param>
		public async Task<IEnumerable<T>> ListDocumentsAsync<T>(string containerName, string queryString)
		{
			Container container = CosmosClient.GetContainer(DatabaseId, containerName);

			FeedIterator<T> query = container.GetItemQueryIterator<T>(new QueryDefinition(queryString));

			List<T> results = new();

			while (query.HasMoreResults)
			{
				FeedResponse<T> response = await query.ReadNextAsync();

				results.AddRange(response.ToList());
			}

			return results;
		}

		/// <summary>
		/// Read selected documents contents.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="documentId">Document id</param>
		public async Task<T> GetDocumentAsync<T>(string containerName, string documentId)
		{
			Container container = CosmosClient.GetContainer(DatabaseId, containerName);

			try
			{
				ItemResponse<T> response = await container.ReadItemAsync<T>(documentId, new PartitionKey(documentId));

				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				throw;
			}
		}

		/// <summary>
		/// Create a document.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="item">Document</param>
		public async Task<T> CreateDocumentAsync<T>(string containerName, T item)
		{
			Container container = CosmosClient.GetContainer(DatabaseId, containerName);

			return await container.CreateItemAsync(item);
		}

		/// <summary>
		/// Replace a document.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="documentId">Document id</param>
		/// <param name="item">Document</param>
		public async Task<T> ReplaceDocumentAsync<T>(string containerName, string documentId, T item)
		{
			Container container = CosmosClient.GetContainer(DatabaseId, containerName);

			return await container.UpsertItemAsync(item, new PartitionKey(documentId));
		}

		/// <summary>
		/// Delete a selected document.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="documentId">Document id</param>
		public async Task<T> DeleteDocumentAsync<T>(string containerName, string documentId)
		{
			Container container = CosmosClient.GetContainer(DatabaseId, containerName);

			return await container.DeleteItemAsync<T>(documentId, new PartitionKey(documentId));
		}
	}
}