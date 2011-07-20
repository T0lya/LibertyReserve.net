using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class AccountNameRequest : Request
	{
		protected const string RequestNodeName = "AccountNameRequest";
		protected const string RequestUrl = BaseApiUrl + "accountname.aspx";
		
		public List<AccountNameOperation> Operations { get; set; }
		
		protected override void CheckRequest()
		{
			base.CheckRequest();
			if (Operations == null || Operations.Count == 0)
				throw new LibertyReserveException("Operation is missing.");
			foreach (AccountNameOperation operation in Operations)
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
		
		public AccountNameResponse GetResponse()
		{
			string response = Send(new Uri(RequestUrl));
			
			return AccountNameResponse.Parse(response);
		}
	}
}
