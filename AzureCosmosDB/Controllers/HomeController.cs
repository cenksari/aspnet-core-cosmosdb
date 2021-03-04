namespace AzureCosmosDB.Controllers
{
	using AzureCosmosDB.Models;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Azure.Documents;
	using Microsoft.Extensions.Configuration;
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	public class HomeController : BaseController
	{
		private const string CollectionId = "persons";

		public HomeController(IConfiguration configuration) : base(configuration)
		{
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			// Create document query
			var querySpec = new SqlQuerySpec
			{
				QueryText = $"SELECT * FROM {CollectionId}"
			};

			// Collect persons
			var results = await CosmosDBRepository.QueryDocuments<Person>(CollectionId, querySpec);

			if (results?.Any() == true)
			{
				ViewBag.Results = results.ToList();
			}

			return View();
		}

		[HttpGet]
		public IActionResult Add() => View();

		[HttpPost]
		public async Task<IActionResult> AddPerson()
		{
			Person person = new()
			{
				Id = Guid.NewGuid().ToString("N"),
				Name = new Name()
				{
					First = Request.Form["first"],
					Last = Request.Form["last"]
				},
				Email = new VerifiedData()
				{
					Data = Request.Form["email"],
					Verified = true,
					DateVerified = DateTime.Now
				},
				Phone = new VerifiedData()
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
				Location = new AddressData()
				{
					City = Request.Form["city"],
					PostCode = Request.Form["postcode"],
					Country = Request.Form["country"],
					Address = Request.Form["address"]
				},
				Login = new Login()
				{
					Username = Request.Form["username"],
					Password = Helpers.Functions.ToMD5(Request.Form["password"]),
					Userkey = Guid.NewGuid().ToString("N")
				}
			};

			await CosmosDBRepository.CreateDocument
			(
				CollectionId,
				person
			);

			return Redirect(Url.Action("", ""));
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			// Create document query
			var querySpec = new SqlQuerySpec
			{
				QueryText = $"SELECT * FROM {CollectionId} t WHERE (t.id=@id)",
				Parameters = new SqlParameterCollection
				{
					new SqlParameter("@id", id)
				}
			};

			// Collect persons
			var results = await CosmosDBRepository.QueryDocuments<Person>(CollectionId, querySpec);

			var result = results.FirstOrDefault();

			if (result != null)
			{
				ViewBag.Results = result;

				return View();
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> EditPerson(string id)
		{
			// Create document query
			var querySpec = new SqlQuerySpec
			{
				QueryText = $"SELECT * FROM {CollectionId} t WHERE (t.id=@id)",
				Parameters = new SqlParameterCollection
				{
					new SqlParameter("@id", id)
				}
			};

			// Collect persons
			var results = await CosmosDBRepository.QueryDocuments<Person>(CollectionId, querySpec);

			var result = results.FirstOrDefault();

			if (result != null)
			{
				result.Name.First = Request.Form["first"];
				result.Name.Last = Request.Form["last"];
				result.Email = new VerifiedData()
				{
					Data = Request.Form["email"]
				};
				result.Phone = new VerifiedData()
				{
					Data = Request.Form["phone"]
				};
				result.Gender = Request.Form["gender"];
				result.Location.City = Request.Form["city"];
				result.Location.PostCode = Request.Form["postcode"];
				result.Location.Country = Request.Form["country"];
				result.Location.Address = Request.Form["address"];
				result.LastUpdated = DateTime.Now;

				await CosmosDBRepository.ReplaceDocument
				(
					CollectionId,
					result.Id,
					result
				);

				return Redirect(Url.Action("", ""));
			}

			return NotFound();
		}

		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			// Create document query
			var querySpec = new SqlQuerySpec
			{
				QueryText = $"SELECT * FROM {CollectionId} t WHERE (t.id=@id)",
				Parameters = new SqlParameterCollection
				{
					new SqlParameter("@id", id)
				}
			};

			// Collect persons
			var results = await CosmosDBRepository.QueryDocuments<Person>(CollectionId, querySpec);

			if (results?.Any() == true)
			{
				string documentId = results.First().Id;

				// Delete person
				await CosmosDBRepository.DeleteDocument(CollectionId, documentId);
			}

			return Redirect(Url.Action("", ""));
		}
	}
}