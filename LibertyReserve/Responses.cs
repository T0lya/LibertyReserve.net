using System;
using System.Collections.Generic;

namespace Magnis.Web.Services.LibertyReserve
{
	public abstract class Response
	{
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
		public List<Balance> Balances { get; protected set; }
		
		public static BalanceResponse Parse(string responseData)
		{
			throw new NotImplementedException();
		}
	}
}

