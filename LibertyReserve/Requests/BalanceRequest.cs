using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class BalanceRequest : Request
	{
		protected const string RequestNodeName = "BalanceRequest";
		protected const string RequestUrl = "https://api.libertyreserve.com/xml/balance.aspx";
		
		public List<BalanceOperation> Operations { get; set; }
				
		protected override void CheckRequest()
		{
			base.CheckRequest();
			if (Operations == null || Operations.Count == 0)
				throw new LibertyReserveException("Operation is missing.");
			foreach (BalanceOperation operation in Operations)
				operation.Check();
		}
		
		public override XElement ToXML()
		{
			return
				new XElement(RequestNodeName, new XAttribute(RequestIdAttributeName, Id),
					Auth.ToXML(),
					Operations.Select(op => op.ToXML()));
		}
		
		public BalanceResponse GetResponse()
		{
			string response = Send(new Uri(RequestUrl));
			
			return BalanceResponse.Parse(response);
		}
	}
}
