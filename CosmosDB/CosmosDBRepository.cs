namespace CosmosDB
{
	using Microsoft.Azure.Documents;
	using Microsoft.Azure.Documents.Client;
	using Microsoft.Azure.Documents.Linq;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Net;
	using System.Threading.Tasks;

	public static class CosmosDBRepository<T> where T : class
	{
		private static DocumentClient client;
		private static readonly string Database = "CosmosDBTest";

		public static void Initialize()
		{
			client = new DocumentClient
			(
				new Uri("https://localhost:8081"),
				"C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
			);

			CreateDatabaseIfNotExistsAsync().Wait();
		}

		public static async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, string collection)
		{
			IDocumentQuery<T> query = client.CreateDocumentQuery<T>
			(
				UriFactory.CreateDocumentCollectionUri
				(
					Database,
					collection
				),
				new FeedOptions { MaxItemCount = -1 }
			)
			.Where(predicate)
			.AsDocumentQuery();

			List<T> results = new List<T>();

			while (query.HasMoreResults)
			{
				results.AddRange(await query.ExecuteNextAsync<T>());
			}

			return results;
		}

		public static async Task<T> GetItemAsync(string id, string collection)
		{
			try
			{
				Document document = await client.ReadDocumentAsync
				(
					UriFactory.CreateDocumentUri
					(
						Database,
						collection,
						id
					)
				);

				return (T)(dynamic)document;
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
				{
					return null;
				}
				else
				{
					throw;
				}
			}
		}

		public static async Task<Document> CreateItemAsync(T item, string collection)
		{
			return await client.CreateDocumentAsync
			(
				UriFactory.CreateDocumentCollectionUri
				(
					Database,
					collection
				),
				item
			);
		}

		public static async Task<Document> UpdateItemAsync(string id, T item, string collection)
		{
			return await client.ReplaceDocumentAsync
			(
				UriFactory.CreateDocumentUri
				(
					Database,
					collection,
					id
				),
				item
			);
		}

		public static async Task DeleteItemAsync(string id, string collection)
		{
			await client.DeleteDocumentAsync
			(
				UriFactory.CreateDocumentUri
				(
					Database,
					collection,
					id
				)
			);
		}

		private static async Task CreateDatabaseIfNotExistsAsync()
		{
			try
			{
				await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(Database));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
				{
					await client.CreateDatabaseAsync(new Database { Id = Database });
				}
				else
				{
					throw;
				}
			}
		}
	}
}