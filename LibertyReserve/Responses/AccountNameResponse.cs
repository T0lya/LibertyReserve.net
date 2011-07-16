using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class AccountName
	{
		protected const string AccountToRetrieveNodeName = "AccountToRetrieve";
		protected const string DateNodeName = "Date";
		protected const string NameNodeName = "Name";

		public string AccountToRetrieve { get; set; }
		public string Name { get; set; }
		public DateTime Date { get; set; }
		
		public static AccountName Parse(XElement xml)
		{
			var accName = new AccountName
			{
				AccountToRetrieve = xml.Element(AccountToRetrieveNodeName).Value.Trim(),
				Name = xml.Element(NameNodeName).Value.Trim(),
				Date = LRConverter.ToDateTime(xml.Element(DateNodeName).Value.Trim()),
			};
			
			return accName;
		}
	}

	
	public class AccountNameResponse : Response
	{
		protected const string AccountNameNodeName = "AccountName";
		
		public List<AccountName> AccountNames { get; set; }

		public AccountNameResponse()
		{
			AccountNames = new List<AccountName>();
		}
		
		public static AccountNameResponse Parse(string responseText)
		{
			try
			{
				XElement xml = XElement.Parse(responseText);
				var response = new AccountNameResponse();
				response.ParseHeader(xml);
				foreach (XElement node in xml.Elements(AccountNameNodeName))
				{
					AccountName accName = AccountName.Parse(node);
					response.AccountNames.Add(accName);
				}
				response.ParseErrors(xml);
				
				return response;
			}
			catch (Exception e)
			{
				throw new LibertyReserveException("Account name response format is invalid.", responseText, e);
			}
		}			
	}
}
