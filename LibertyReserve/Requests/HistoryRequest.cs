using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class HistoryRequest : Request
	{
		protected const string RequestNodeName = "HistoryRequest";
		protected const string RequestUrl = "https://api.libertyreserve.com/xml/history.aspx";
		
		public List<HistoryOperation> Operations { get; set; }
				
		public override XElement ToXML()
		{
			return
				new XElement(RequestNodeName, new XAttribute(RequestIdAttributeName, Id),
					Auth.ToXML(),
					Operations.Select(op => op.ToXML()));
		}
		
		public HistoryResponse GetResponse()
		{
			string response = Send(new Uri(RequestUrl));
			
			return HistoryResponse.Parse(response);
		}
	}
}

