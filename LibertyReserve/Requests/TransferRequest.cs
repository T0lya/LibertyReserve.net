using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class TransferRequest : Request
	{
		protected const string RequestNodeName = "TransferRequest";
		protected const string RequestUrl = BaseApiUrl + "transfer.aspx";
		
		public List<TransferOperation> Operations { get; set; }
		
		protected override void CheckRequest()
		{
			base.CheckRequest();
			if (Operations == null || Operations.Count == 0)
				throw new LibertyReserveException("Operation is missing.");
			foreach (TransferOperation operation in Operations)
				operation.Check();
		}
		
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

