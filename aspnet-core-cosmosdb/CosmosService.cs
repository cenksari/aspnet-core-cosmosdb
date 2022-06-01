namespace Tools
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
	public class CosmosService : ICosmosService
	{
		private readonly CosmosClient CosmosClient;

		private const string DatabaseId = "apis"; // ENTER YOUR COSMOS DB NAME

		private readonly CosmosClientOptions options = new()
		{
			PortReuseMode = PortReuseMode.PrivatePortPool,
			SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase }
		};

		public CosmosService(IConfiguration configuration) => CosmosClient = new(configuration["CosmosDbEndpoint"], configuration["CosmosDbKey"], options);

		/// <summary>
		/// Read selected documents contents.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="documentId">Document id</param>
		public async Task<T?> GetDocumentAsync<T>(string containerName, string documentId)
		{
			Container container = CosmosClient.GetContainer(DatabaseId, containerName);

			try
			{
				ItemResponse<T> response = await container.ReadItemAsync<T>(documentId, new PartitionKey(documentId));

				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return default(T);
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
		/// Return all documents in selected collection.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="queryString">Document query</param>
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
		/// Replace a document.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="documentId">Document id</param>
		/// <param name="item">Document</param>
		public async Task<T?> ReplaceDocumentAsync<T>(string containerName, string documentId, T item)
		{
			if (!string.IsNullOrWhiteSpace(documentId))
			{
				Container container = CosmosClient.GetContainer(DatabaseId, containerName);

				return await container.UpsertItemAsync(item, new PartitionKey(documentId));
			}

			return default(T);
		}

		/// <summary>
		/// Delete a selected document.
		/// </summary>
		/// <param name="containerName">Container name</param>
		/// <param name="documentId">Document id</param>
		public async Task<T?> DeleteDocumentAsync<T>(string containerName, string documentId)
		{
			if (!string.IsNullOrWhiteSpace(documentId))
			{
				Container container = CosmosClient.GetContainer(DatabaseId, containerName);

				return await container.DeleteItemAsync<T>(documentId, new PartitionKey(documentId));
			}

			return default(T);
		}
	}
}