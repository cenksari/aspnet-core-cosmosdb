namespace CosmosDB.Controllers
{
	using CosmosDB.Helpers;
	using CosmosDB.Models;
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Web.Mvc;

	public class HomeController : Controller
	{
		public async Task<ActionResult> Index()
		{
			var persons = await CosmosDBRepository<Person>.GetItemsAsync(p => p.id != string.Empty, "Members");

			int rc = persons.Count();

			if (rc == 0)
			{
				return View();
			}
			else
			{
				ViewBag.Results = rc;

				return View(persons);
			}
		}

		public async Task<JsonResult> ListRecords()
		{
			var persons = await CosmosDBRepository<Person>.GetItemsAsync(p => p.id != string.Empty, "Members");

			return Json(persons, JsonRequestBehavior.AllowGet);
		}

		public async Task<ActionResult> Add()
		{
			if (Request.HttpMethod == "POST")
			{
				Person person = new Person
				{
					name = new Name()
					{
						first = Request.Form["first"],
						last = Request.Form["last"]
					},
					email = new VerifiedData()
					{
						data = Request.Form["email"],
						verified = true,
						dateverified = DateTime.Now
					},
					phone = new VerifiedData()
					{
						data = Request.Form["phone"],
						verified = true,
						dateverified = DateTime.Now
					},
					gender = Request.Form["gender"],
					localization = "tr-TR",
					dateofbirth = new DateTime(1980, 5, 2).Date,
					active = true,
					registered = DateTime.Now,
					lastupdated = DateTime.Now,
					location = new Address()
					{
						city = Request.Form["city"],
						postcode = Request.Form["postcode"],
						country = Request.Form["country"],
						address = Request.Form["address"]
					},
					login = new Login()
					{
						username = Request.Form["username"],
						password = Request.Form["password"].ToMD5(),
						userkey = Guid.NewGuid().ToString("N")
					}
				};

				await CosmosDBRepository<Person>.CreateItemAsync(person, "Members");

				return Redirect(Url.Action("", ""));
			}

			return View();
		}

		public async Task<ActionResult> Edit(string id)
		{
			Person person = await CosmosDBRepository<Person>.GetItemAsync(id, "Members");

			if (person == null)
			{
				return HttpNotFound();
			}
			else
			{
				if (Request.HttpMethod == "POST")
				{
					person.name.first = Request.Form["first"];
					person.name.last = Request.Form["last"];
					person.email = new VerifiedData()
					{
						data = Request.Form["email"]
					};
					person.phone = new VerifiedData()
					{
						data = Request.Form["phone"]
					};
					person.gender = Request.Form["gender"];
					person.location.city = Request.Form["city"];
					person.location.postcode = Request.Form["postcode"];
					person.location.country = Request.Form["country"];
					person.location.address = Request.Form["address"];
					person.lastupdated = DateTime.Now;

					await CosmosDBRepository<Person>.UpdateItemAsync(id, person, "Members");

					return Redirect(Url.Action("", ""));
				}
			}

			return View(person);
		}

		public async Task<ActionResult> Delete(string id)
		{
			Person person = await CosmosDBRepository<Person>.GetItemAsync(id, "Members");

			if (person == null)
			{
				return HttpNotFound();
			}
			else
			{
				await CosmosDBRepository<Person>.DeleteItemAsync(id, "Members");

				return Redirect(Url.Action("", ""));
			}
		}
	}
}