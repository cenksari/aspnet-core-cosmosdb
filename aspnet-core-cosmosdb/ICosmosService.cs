namespace aspnet_core_cosmosdb;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICosmosService
{
	Task<T> CreateDocumentAsync<T>(string containerName, T item);

	Task<T?> GetDocumentAsync<T>(string containerName, string documentId);

	Task<bool> DeleteDocumentAsync<T>(string containerName, string documentId);

	Task<T?> ReplaceDocumentAsync<T>(string containerName, string documentId, T item);

	Task<IEnumerable<T>?> ListDocumentsAsync<T>(string containerName, string queryString);
}