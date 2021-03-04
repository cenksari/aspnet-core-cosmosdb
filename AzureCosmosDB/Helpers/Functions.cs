namespace AzureCosmosDB.Helpers
{
	using System.Security.Cryptography;
	using System.Text;

	/// <summary>
	/// General helpers.
	/// </summary>
	public static class Functions
	{
		/// <summary>
		/// Convert string to MD5 format.
		/// </summary>
		/// <returns>Encrypted string</returns>
		/// <param name="value">String to convert</param>
		public static string ToMD5(this string value)
		{
			var byteData = Encoding.ASCII.GetBytes(value);

			var oMd5 = MD5.Create();

			var hashData = oMd5.ComputeHash(byteData);

			var sb = new StringBuilder();

			for (int x = 0; x < hashData.Length; x++)
			{
				sb.Append(hashData[x].ToString("x2"));
			}

			return sb.ToString();
		}
	}
}