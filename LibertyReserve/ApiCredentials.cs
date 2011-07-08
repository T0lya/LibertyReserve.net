using System;

namespace Magnis.Web.Services.LibertyReserve
{
	public class ApiCredentials
	{
		public string AccountNumber { get; set; }
		public string ApiName { get; set; }
		public string SecurityWord { get; set; }
		
		#region Constructors
		
		public ApiCredentials ()
		{
		}
		
		public ApiCredentials(string accountNumber, string apiName, string securityWord)
		{
			AccountNumber = accountNumber;
			ApiName = apiName;
			SecurityWord = securityWord;
		}
		
		#endregion
	}
}