using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class Balance
	{
		protected const string AccountIdNodeName = "AccountId";
		protected const string CurrencyIdNodeName = "CurrencyId";
		protected const string DateNodeName = "Date";
		protected const string ValueNodeName = "Value";

		public string AccountId { get; set; }
		public Currency Currency { get; set; }
		public DateTime Timestamp { get;set; }
		public double Value { get; set; }
		
		public static Balance Parse(XElement xml)
		{
			var balance = new Balance
			{
				AccountId = xml.Element(AccountIdNodeName).Value.Trim(),
				Currency = LRConverter.ToCurrency(xml.Element(CurrencyIdNodeName).Value.Trim()),
				Value = LRConverter.ToDouble(xml.Element(ValueNodeName).Value),
				Timestamp = LRConverter.ToDateTime(xml.Element(DateNodeName).Value.Trim()),
			};
			
			return balance;
		}
	}

	
	public class BalanceResponse : Response
	{
		protected const string BalanceNodeName = "Balance";
		
		public List<Balance> Balances { get; protected set; }
		
		public BalanceResponse()
		{
			Balances = new List<Balance>();
		}
		
		public static BalanceResponse Parse(string responseText)
		{
			try
			{
				XElement xml = XElement.Parse(responseText);
				var response = new BalanceResponse();
				response.ParseHeader(xml);
				foreach (XElement node in xml.Elements(BalanceNodeName))
				{
					Balance b = Balance.Parse(node);
					response.Balances.Add(b);
				}
				response.ParseErrors(xml);
				
				return response;
			}
			catch (Exception e)
			{
				throw new LibertyReserveException("Balance response format is invalid.", responseText, e);
			}
		}
	}
}
