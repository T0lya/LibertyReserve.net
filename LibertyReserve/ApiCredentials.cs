using System;
using System.Security.Cryptography;
using System.Text;

namespace Magnis.Web.Services.LibertyReserve
{
	public sealed class ApiCredentials
	{
		public string AccountNumber { get; set; }
		public string ApiName { get; set; }
		public string SecurityWord { get; set; }
		
		public ApiCredentials ()
		{
		}
		
		public ApiCredentials(string accountNumber, string apiName, string securityWord)
		{
			AccountNumber = accountNumber;
			ApiName = apiName;
			SecurityWord = securityWord;
		}
		
		public string CreateSecurityToken()
		{
			string data = String.Format("{0}:{1:yyyyMMdd:HH}", SecurityWord, DateTime.Now);
			using (var sha = new SHA256Managed())
			{
				byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
				string token = BitConverter.ToString(hash).Replace('-', ' ');
				
				return token;
			}
		}
	}
}

