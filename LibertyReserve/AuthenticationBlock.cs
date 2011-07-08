using System;
using System.Security.Cryptography;
using System.Text;

namespace Magnis.Web.Services.LibertyReserve
{
	public class AuthenticationBlock
	{
		public string ApiName { get; set; }
		public string Token { get; set; }
		
		#region Constructors
		
		public AuthenticationBlock()
		{
		}
		
		public AuthenticationBlock(string apiName, string token)
		{
			ApiName = apiName;
			Token = token;
		}
		
		#endregion
		
		public static AuthenticationBlock FromApiCredentials(ApiCredentials credentials)
		{
			string token = CreateSecurityToken(credentials.SecurityWord);
			var auth = new AuthenticationBlock(credentials.ApiName, token);
			
			return auth;
		}
		
		protected static string CreateSecurityToken(string securityWord)
		{
			string data = String.Format("{0}:{1:yyyyMMdd:HH}", securityWord, DateTime.Now);
			using (var sha = new SHA256Managed())
			{
				byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
				string token = BitConverter.ToString(hash).Replace('-', ' ');
				
				return token;
			}
		}
	}
}

