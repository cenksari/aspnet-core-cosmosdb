namespace AzureCosmosDB.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;

	public abstract class BaseController : Controller
	{
		public readonly CosmosDBRepository CosmosDBRepository;

		protected BaseController(IConfiguration configuration)
		{
			CosmosDBRepository = new(configuration);
		}
	}
}