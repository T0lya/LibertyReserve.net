using System;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class BalanceOperation
	{
		protected const string OperationNodeName = "Balance";
		protected const string CurrencyIdNodeName = "CurrencyId";
		protected const string AccountIdNodeName = "AccountId";
		
		public Currency Currency { get; set; }
		public string AccountId { get; set; }
		
		public XElement ToXML()
		{
			return
				new XElement(OperationNodeName,
					new XElement(CurrencyIdNodeName, LRConverter.ToString(Currency)),
					new XElement(AccountIdNodeName, AccountId)
				);
		}
	}
	
	
	public class AccountNameOperation
	{
		protected const string OperationNodeName = "Account";
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

