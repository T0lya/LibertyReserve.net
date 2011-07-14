using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class AccountName
	{
		public string AccountToRetrieve { get; set; }
		public string Name { get; set; }
		public DateTime Date { get; set; }
	}

	
	public class AccountNameResponse : Response
	{
		protected const string AccountNameResponseNodeName = "AccountNameResponse";
		protected const string AccountNameNodeName = "AccountName";
		protected const string AccountToRetrieveNodeName = "AccountToRetrieve";
		protected const string DateNodeName = "Date";
		protected const string NameNodeName = "Name";
		
		public List<AccountName> AccountNames { get; set; }
		
		public static AccountNameResponse Parse(string responseText)
		{
			try
			{
				XElement xml = XElement.Parse(responseText);
				var response = new AccountNameResponse
				{
					ResponseText = xml.ToString(),
					RequestId = xml.Attribute(RequestIdAttributeName).Value,
					Timestamp = LRConverter.ToDateTime(xml.Attribute(ResponseDateAttributeName).Value),
					AccountNames = new List<AccountName>(),
				};
				foreach (XElement node in xml.Elements(AccountNameNodeName))
				{
					var account = new AccountName
					{
						AccountToRetrieve = node.Element(AccountToRetrieveNodeName).Value.Trim(),
						Name = node.Element(NameNodeName).Value.Trim(),
						Date = LRConverter.ToDateTime(node.Element(DateNodeName).Value.Trim()),
					};
					response.AccountNames.Add(account);
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
