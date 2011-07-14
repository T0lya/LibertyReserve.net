using System;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class AccountNameOperation
	{
		protected const string OperationNodeName = "AccountName";
		protected const string AccountIdNodeName = "AccountId";
		protected const string AccountToRetrieveNodeName = "AccountToRetrieve";
		
		public string AccountId { get; set; }
		public string AccountToRetrieve { get; set; }
		
		public XElement ToXML()
		{
			return
				new XElement(OperationNodeName,
					new XElement(AccountIdNodeName, AccountId),
					new XElement(AccountToRetrieveNodeName, AccountToRetrieve)
				);
		}
	}
}
