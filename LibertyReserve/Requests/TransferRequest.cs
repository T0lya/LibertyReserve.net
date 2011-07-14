using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class TransferRequest : Request
	{
		protected const string RequestNodeName = "TransferRequest";
		protected const string TransferNodeName = "Transfer";
		protected const string RequestUrl = "https://api.libertyreserve.com/xml/transfer.aspx";
		
		public List<TransferOperation> Operations { get; set; }
		
		public override XElement ToXML()
		{
			return
				new XElement(RequestNodeName, new XAttribute(RequestIdAttributeName, Id),
					Auth.ToXML(),
					Operations.Select(op => op.ToXML())
				);
		}
		
		public TransferResponse GetResponse()
		{
			string response = Send(new Uri(RequestUrl));
			
			return TransferResponse.Parse(response);
		}
	}
}

