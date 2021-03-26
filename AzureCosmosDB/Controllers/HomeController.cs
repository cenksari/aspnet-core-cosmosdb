namespace AzureCosmosDB.Controllers
{
	using AzureCosmosDB.Models;
	using AzureCosmosDB.Services;
	using Microsoft.AspNetCore.Mvc;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class HomeController : BaseController
	{
		private const string CollectionId = "persons";

		public HomeController(ICosmosDBService cosmosDBService) : base(cosmosDBService)
		{
		}

		/// <summary>
		/// List all persons.
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			// Create document query
			const string query = "SELECT * FROM c";

			// Collect persons
			IEnumerable<Person> results = await CosmosDBService.ListDocumentsAsync<Person>(CollectionId, query);

			if (results?.Any() == true)
			{
				ViewBag.Results = results.ToList();
			}

			return View();
		}

		/// <summary>
		/// Add new person form.
		/// </summary>
		[HttpGet]
		public IActionResult Add() => View();

		/// <summary>
		/// Add new person to db.
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> AddPerson()
		{
			Person person = new()
			{
				Id = Guid.NewGuid().ToString(),
				Name = new()
				{
					First = Request.Form["first"],
					Last = Request.Form["last"]
				},
				Email = new()
				{
					Data = Request.Form["email"],
					Verified = true,
					DateVerified = DateTime.Now
				},
				Phone = new()
				{
					Data = Request.Form["phone"],
					Verified = true,
					DateVerified = DateTime.Now
				},
				Gender = Request.Form["gender"],
				Localization = "tr-TR",
				DateOfBirth = new DateTime(1980, 5, 2).Date,
				Active = true,
				Registered = DateTime.Now,
				LastUpdated = DateTime.Now,
				Location = new()
				{
					City = Request.Form["city"],
					PostCode = Request.Form["postcode"],
					Country = Request.Form["country"],
					Address = Request.Form["address"]
				},
				Login = new()
				{
					Username = Request.Form["username"],
					Password = Helpers.Functions.ToMD5(Request.Form["password"]),
					Userkey = Guid.NewGuid().ToString("N")
				}
			};

			await CosmosDBService.CreateDocumentAsync
			(
				CollectionId,
				person
			);

			return Redirect(Url.Action("", ""));
		}

		/// <summary>
		/// Edit person form.
		/// </summary>
		/// <param name="id">Document ID</param>
		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			// Create document query
			string query = $"SELECT * FROM c WHERE (c.id=\"{id}\")";

			// Collect persons
			IEnumerable<Person> results = await CosmosDBService.ListDocumentsAsync<Person>(CollectionId, query);

			if (results?.Any() == true)
			{
				Person result = results.FirstOrDefault();

				ViewBag.Results = result;

				return View();
			}

			return NotFound();
		}

		/// <summary>
		/// Edit person.
		/// </summary>
		/// <param name="id">Document ID</param>
		[HttpPost]
		public async Task<IActionResult> EditPerson(string id)
		{
			// Create document query
			string query = $"SELECT * FROM c WHERE (c.id=\"{id}\")";

			// Collect persons
			IEnumerable<Person> results = await CosmosDBService.ListDocumentsAsync<Person>(CollectionId, query);

			if (results?.Any() == true)
			{
				Person result = results.FirstOrDefault();

				result.Name.First = Request.Form["first"];
				result.Name.Last = Request.Form["last"];
				result.Email = new()
				{
					Data = Request.Form["email"]
				};
				result.Phone = new()
				{
					Data = Request.Form["phone"]
				};
				result.Gender = Request.Form["gender"];
				result.Location.City = Request.Form["city"];
				result.Location.PostCode = Request.Form["postcode"];
				result.Location.Country = Request.Form["country"];
				result.Location.Address = Request.Form["address"];
				result.LastUpdated = DateTime.Now;

				await CosmosDBService.ReplaceDocumentAsync
				(
					CollectionId,
					result.Id,
					result
				);

				return Redirect(Url.Action("", ""));
			}

			return NotFound();
		}

		/// <summary>
		/// Delete person.
		/// </summary>
		/// <param name="id">Document ID</param>
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			// Create document query
			string query = $"SELECT * FROM c WHERE (c.id=\"{id}\")";

			// Collect persons
			IEnumerable<Person> results = await CosmosDBService.ListDocumentsAsync<Person>(CollectionId, query);

			if (results?.Any() == true)
			{
				string documentId = results.First().Id;

				// Delete person
				await CosmosDBService.DeleteDocumentAsync<Person>(CollectionId, documentId);
			}

			return Redirect(Url.Action("", ""));
		}
	}
}