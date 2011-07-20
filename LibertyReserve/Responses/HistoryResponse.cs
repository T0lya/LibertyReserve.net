using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class HistoryResponse : Response
	{
		public Pager Pager { get; protected set; }
		public List<Receipt> Receipts { get; protected set; }
		
		public HistoryResponse()
		{
			Receipts = new List<Receipt>();
		}
		
		public static HistoryResponse Parse(string responseText)
		{
			try
			{
				XElement xml = XElement.Parse(responseText);
				var response = new HistoryResponse();
				response.ParseHeader(xml);
				XElement pagerXml = xml.Element(Pager.PagerNodeName);
				if (pagerXml != null)
					response.Pager = Pager.Parse(pagerXml);
				foreach (XElement node in xml.Elements(Receipt.ReceiptNodeName))
				{
					Receipt r = Receipt.Parse(node);
					response.Receipts.Add(r);
				}
				response.ParseErrors(xml);
				
				return response;
			}
			catch (Exception e)
			{
				throw new LibertyReserveException("History response format is invalid.", responseText, e);
			}
		}
	}
}

