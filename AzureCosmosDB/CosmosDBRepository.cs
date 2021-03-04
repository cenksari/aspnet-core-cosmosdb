namespace AzureCosmosDB.Controllers
{
	using Microsoft.Azure.Documents;
	using Microsoft.Azure.Documents.Client;
	using Microsoft.Azure.Documents.Linq;
	using Microsoft.Extensions.Configuration;
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;

	/// <summary>
	/// Document DB helpers.
	/// </summary>
	public class CosmosDBRepository
	{
		private readonly DocumentClient Client;

		private const string DatabaseId = "cosmosdbtest";

		/// <summary>
		/// Initialize database helper.
		/// </summary>
		public CosmosDBRepository(IConfiguration config)
		{
			if (Client == null)
				Client = new(new Uri(config["CosmosDBUri"]), config["CosmosDBKey"]);
		}

		/// <summary>
		/// Return all documents in selected collection.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		public async Task<IEnumerable<T>> ListDocuments<T>(string collectionId)
		{
			var results = new List<T>();

			IDocumentQuery<T> query = Client.CreateDocumentQuery<T>
			(
				GetCollectionUri(collectionId), new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true }
			)
			.AsDocumentQuery();

			while (query.HasMoreResults)
				results.AddRange(await query.ExecuteNextAsync<T>());

			return results;
		}

		/// <summary>
		/// Return all documents of query results.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		/// <param name="sqlQuerySpec">DocumentDB SQL querySpec</param>
		public async Task<IEnumerable<T>> QueryDocuments<T>(string collectionId, SqlQuerySpec sqlQuerySpec)
		{
			var results = new List<T>();

			var query = Client.CreateDocumentQuery<T>
			(
				GetCollectionUri(collectionId), sqlQuerySpec, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true }
			)
			.AsDocumentQuery();

			while (query.HasMoreResults)
				results.AddRange(await query.ExecuteNextAsync<T>());

			return results;
		}

		/// <summary>
		/// Read selected documents contents.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		/// <param name="documentId">Document id</param>
		public async Task<T> GetDocument<T>(string collectionId, string documentId)
		{
			try
			{
				var document = await Client.ReadDocumentAsync
				(
					GetDocumentUri(collectionId, documentId),
					new RequestOptions { PartitionKey = new PartitionKey(documentId) }
				);

				return (T)(dynamic)document;
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					return (T)(dynamic)null;

				throw;
			}
		}

		/// <summary>
		/// Create a document.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		/// <param name="item">Document</param>
		public async Task<Document> CreateDocument<T>(string collectionId, T item)
		{
			return await Client.CreateDocumentAsync(GetCollectionUri(collectionId), item);
		}

		/// <summary>
		/// Replace a document.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		/// <param name="documentId">Document id</param>
		/// <param name="item">Document</param>
		public async Task<Document> ReplaceDocument<T>(string collectionId, string documentId, T item)
		{
			return await Client.ReplaceDocumentAsync(GetDocumentUri(collectionId, documentId), item, new RequestOptions { PartitionKey = new PartitionKey(documentId) });
		}

		/// <summary>
		/// Delete a selected document.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		/// <param name="documentId">Document id</param>
		public async Task<bool> DeleteDocument(string collectionId, string documentId)
		{
			try
			{
				await Client.DeleteDocumentAsync
				(
					GetDocumentUri(collectionId, documentId),
					new RequestOptions { PartitionKey = new PartitionKey(documentId) }
				);

				return true;
			}
			catch (DocumentClientException)
			{
				return false;
			}
		}

		/// <summary>
		/// Create collection uri.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		private static Uri GetCollectionUri(string collectionId)
		{
			return UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId);
		}

		/// <summary>
		/// Create document uri.
		/// </summary>
		/// <param name="collectionId">Collection id</param>
		/// <param name="documentId">Document id</param>
		private static Uri GetDocumentUri(string collectionId, string documentId)
		{
			return UriFactory.CreateDocumentUri(DatabaseId, collectionId, documentId);
		}
	}
}