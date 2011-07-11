using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace Magnis.Web.Services.LibertyReserve
{
	public abstract class Response
	{
		protected const string RequestIdAttributeName = "id";
		protected const string ResponseDateAttributeName = "date";
		
		public string ResponseText { get; set; }
		public string RequestId { get; set; }
		public DateTime Timestamp { get; set; }
	}
	
	
	public class Balance
	{
		public string AccountId { get; set; }
		public Currency Currency { get; set; }
		public DateTime Timestamp { get;set; }
		public double Value { get; set; }
	}
	
	
	public class BalanceResponse : Response
	{
		protected const string BalanceResponseNodeName = "BalanceResponse";
		protected const string BalanceNodeName = "Balance";
		protected const string AccountIdNodeName = "AccountId";
		protected const string CurrencyIdNodeName = "CurrencyId";
		protected const string DateNodeName = "Date";
		protected const string ValueNodeName = "Value";
		
		public List<Balance> Balances { get; protected set; }
		
		public static BalanceResponse Parse(string responseData)
		{
			XElement xml = XElement.Parse(responseData);
			var response = new BalanceResponse
			{
				ResponseText = xml.ToString(),
				RequestId = xml.Attribute(RequestIdAttributeName).Value,
				Timestamp = LRConverter.ToDateTime(xml.Attribute(ResponseDateAttributeName).Value),
				Balances = new List<Balance>(),
			};
			foreach (XElement node in xml.Elements(BalanceNodeName))
			{
				var b = new Balance
				{
					AccountId = node.Element(AccountIdNodeName).Value.Trim(),
					Currency = LRConverter.FromString(node.Element(CurrencyIdNodeName).Value.Trim()),
					Value = Double.Parse(node.Element(ValueNodeName).Value, CultureInfo.InvariantCulture),
					Timestamp = LRConverter.ToDateTime(node.Element(DateNodeName).Value.Trim()),
				};
				response.Balances.Add(b);
			}
			
			return response;
		}
	}
}

