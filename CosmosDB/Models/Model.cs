namespace CosmosDB.Models
{
	using Newtonsoft.Json;
	using System;

	public class Person
	{
		public string id { get; set; }
		public bool active { get; set; }
		public string gender { get; set; }
		public Name name { get; set; }
		public Login login { get; set; }
		public Address location { get; set; }
		public VerifiedData email { get; set; }
		public VerifiedData phone { get; set; }
		public ProfilePicture picture { get; set; }
		public DateTime dateofbirth { get; set; }
		public string localization { get; set; }
		public DateTime registered { get; set; }
		public DateTime lastupdated { get; set; }
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}

	public class Name
	{
		public string first { get; set; }
		public string last { get; set; }
	}

	public class Address
	{
		public string city { get; set; }
		public string country { get; set; }
		public string address { get; set; }
		public string postcode { get; set; }
	}

	public class VerifiedData
	{
		public string data { get; set; }
		public bool verified { get; set; }
		public DateTime dateverified { get; set; }
	}

	public class Login
	{
		public string username { get; set; }
		public string password { get; set; }
		public string userkey { get; set; }
	}

	public class ProfilePicture
	{
		public string large { get; set; }
		public string medium { get; set; }
		public string thumbnail { get; set; }
	}
}