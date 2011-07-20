using System;
using System.Linq;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class HistoryRequest : Request
	{
		protected const string RequestNodeName = "HistoryRequest";
		protected const string RequestUrl = BaseApiUrl + "history.aspx";
		
		public HistoryOperation Operation { get; set; }
				
		protected override void CheckRequest()
		{
			base.CheckRequest();
			if (Operation == null)
				throw new LibertyReserveException("Operation is missing.");
			Operation.Check();
		}
		
		public override XElement ToXML()
		{
			return
				new XElement(RequestNodeName, new XAttribute(RequestIdAttributeName, Id),
					Auth.ToXML(),
					Operation.ToXML());
		}
		
		public HistoryResponse GetResponse()
		{
			string response = Send(new Uri(RequestUrl));
			
			return HistoryResponse.Parse(response);
		}
	}
}

