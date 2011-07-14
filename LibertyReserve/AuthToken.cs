using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class AuthToken
	{
		protected const string AuthNodeName = "Auth";
		protected const string ApiNameNodeName = "ApiName";
		protected const string TokenNodeName = "Token";
		
		public string ApiName { get; set; }
		public string Token { get; set; }
		
		#region Constructors
		
		public AuthToken()
		{
		}
		
		public AuthToken(string apiName, string token)
		{
			ApiName = apiName;
			Token = token;
		}
		
		#endregion
		
		public static AuthToken FromApiCredentials(ApiCredentials credentials)
		{
			string token = CreateSecurityToken(credentials.SecurityWord);
			var auth = new AuthToken(credentials.ApiName, token);
			
			return auth;
		}
		
		protected static string CreateSecurityToken(string securityWord)
		{
			string data = String.Format("{0}:{1:yyyyMMdd:HH}", securityWord, DateTime.UtcNow);
			using (var sha = new SHA256Managed())
			{
				byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
				string token = BitConverter.ToString(hash).Replace("-", String.Empty);
				
				return token;
			}
		}
		
		public XElement ToXML()
		{
			return 	
				new XElement(AuthNodeName, 
					new XElement(ApiNameNodeName, ApiName),
					new XElement(TokenNodeName, Token)
				);
		}
	}
}

