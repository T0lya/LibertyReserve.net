using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public abstract class Request
	{
		protected const string RequestIdAttributeName = "id";
		
		public string Id { get; set; }
		public AuthenticationBlock Auth { get; set; }
	}
	
	
	public class BalanceRequest : Request
	{
		protected const string RequestNodeName = "BalanceRequest";
		protected const string BalanceNodeName = "Balance";
		
		public List<BalanceOperation> Operations { get; set; }
		
		#region Constructors
		
		public BalanceRequest ()
		{
		}
		
		#endregion
		
		public XElement ToXML()
		{
			return
				new XElement(RequestNodeName,
					Auth.ToXML(),
					Operations.Select(op => op.ToXML()));
		}
	}
}

