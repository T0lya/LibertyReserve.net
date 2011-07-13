using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace Magnis.Web.Services.LibertyReserve
{
	public class ApiError
	{
		protected const string CodeNodeName = "Code";
		protected const string TextNodeName = "Text";
		protected const string DescriptionNodeName = "Description";
		
		public int Code { get; set; }
		public string Text { get; set; }
		public string Description { get; set; }
		
		public static ApiError Parse(XElement xml)
		{
			var error = new ApiError
			{
				Code = Int32.Parse(xml.Element(CodeNodeName).Value),
				Text = xml.Element(TextNodeName).Value.Trim(),
				Description = xml.Element(DescriptionNodeName).Value.Trim(),
			};
			
			return error;
		}
	}
	
	
	public abstract class Response
	{
		protected const string RequestIdAttributeName = "id";
		protected const string ResponseDateAttributeName = "date";
		protected const string ErrorNodeName = "Error";
		
		public string ResponseText { get; set; }
		public string RequestId { get; set; }
		public DateTime Timestamp { get; set; }
		public List<ApiError> Errors { get; protected set; }
		
		protected void ParseErrors(XElement xml)
		{
			Errors = new List<ApiError>();
			foreach (XElement node in xml.Elements(ErrorNodeName))
			{
				Errors.Add(ApiError.Parse(node));
			}
		}
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
		
		public static BalanceResponse Parse(string responseText)
		{
			try
			{
				XElement xml = XElement.Parse(responseText);
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
				response.ParseErrors(xml);
				
				return response;
			}
			catch (Exception e)
			{
				throw new LibertyReserveException("Balance response format is invalid.", responseText, e);
			}
		}
	}
	
	
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

