namespace AzureCosmosDB.Services
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface ICosmosDBService
	{
		Task<IEnumerable<T>> ListDocumentsAsync<T>(string containerName, string queryString);

		Task<T> GetDocumentAsync<T>(string containerName, string documentId);

		Task<T> CreateDocumentAsync<T>(string containerName, T item);

		Task<T> ReplaceDocumentAsync<T>(string containerName, string documentId, T item);

		Task<T> DeleteDocumentAsync<T>(string containerName, string documentId);
	}
}