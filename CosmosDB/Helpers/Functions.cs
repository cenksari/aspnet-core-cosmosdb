namespace CosmosDB.Helpers
{
	using System.Security.Cryptography;
	using System.Text;

	public static class Functions
	{
		/// <summary>
		/// Gönderilen metni MD5 formatında kriptolar.
		/// </summary>
		/// <param name="encryptedString">Kriptolanacak metin</param>
		public static string ToMD5(this string encryptedString)
		{
			string returnedValue = string.Empty;

			if (!string.IsNullOrEmpty(encryptedString))
			{
				byte[] byteData = Encoding.ASCII.GetBytes(encryptedString);
				MD5 oMd5 = MD5.Create();
				byte[] hashData = oMd5.ComputeHash(byteData);
				StringBuilder sb = new StringBuilder();
				int x = 0;

				while (x < hashData.Length)
				{
					sb.Append(hashData[x].ToString("x2"));
					x += 1;
				}

				returnedValue = sb.ToString();
			}

			return returnedValue;
		}
	}
}