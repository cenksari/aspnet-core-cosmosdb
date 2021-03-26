namespace AzureCosmosDB.Controllers
{
	using AzureCosmosDB.Services;
	using Microsoft.AspNetCore.Mvc;

	public abstract class BaseController : Controller
	{
		public readonly ICosmosDBService CosmosDBService;

		protected BaseController(ICosmosDBService cosmosDBService) => CosmosDBService = cosmosDBService;
	}
}