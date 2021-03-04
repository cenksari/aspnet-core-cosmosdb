namespace AzureCosmosDB.Models
{
	using System;

	public class Person
	{
		public string Id { get; set; }
		public bool Active { get; set; }
		public string Gender { get; set; }
		public Name Name { get; set; }
		public Login Login { get; set; }
		public AddressData Location { get; set; }
		public VerifiedData Email { get; set; }
		public VerifiedData Phone { get; set; }
		public ProfilePicture Picture { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Localization { get; set; }
		public DateTime Registered { get; set; }
		public DateTime LastUpdated { get; set; }
	}

	public class Name
	{
		public string First { get; set; }
		public string Last { get; set; }
	}

	public class AddressData
	{
		public string City { get; set; }
		public string Country { get; set; }
		public string Address { get; set; }
		public string PostCode { get; set; }
	}

	public class VerifiedData
	{
		public string Data { get; set; }
		public bool Verified { get; set; }
		public DateTime DateVerified { get; set; }
	}

	public class Login
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Userkey { get; set; }
	}

	public class ProfilePicture
	{
		public string Large { get; set; }
		public string Medium { get; set; }
		public string Thumbnail { get; set; }
	}
}