using System;
using System.Collections.Generic;

namespace Magnis.Web.Services.LibertyReserve
{
	public abstract class Request
	{
		public string Id { get; set; }
		public AuthenticationBlock Auth { get; set; }
	}
	
	
	public class BalanceRequest : Request
	{
		public List<BalanceOperation> Operations { get; set; }
		
		public BalanceRequest ()
		{
		}
	}
}

