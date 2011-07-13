using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public abstract class Request
	{
		protected const int DefaultTimeout = 60 * 1000;
		protected const string RequestIdAttributeName = "id";
		
		public int Timeout { get; set; }
		public Encoding Encoding { get; set; }
		public string Id { get; set; }
		public AuthenticationBlock Auth { get; set; }
		
		public Request()
		{
			Timeout = DefaultTimeout;
			Encoding = Encoding.UTF8;
		}
		
		public abstract XElement ToXML();
		
		public string Send(Uri url)
		{
			HttpWebRequest request = CreateWebRequest(url);
			using (WebResponse response = request.GetResponse())
			{
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					return reader.ReadToEnd();
				}
			}
		}
		
		protected HttpWebRequest CreateWebRequest(Uri url)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = "POST";
			request.KeepAlive = false;
			request.ContentType = "application/x-www-form-urlencoded";
			request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";
			request.Timeout = Timeout;
			
			string requestData = ToXML().ToString();
			byte[] requestBytes = Encoding.GetBytes("req=" + HttpUtility.UrlEncode(requestData));
			request.ContentLength = requestBytes.Length;
			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(requestBytes, 0, requestBytes.Length);
			}
			
			return request;
		}
	}
	
	
	public class BalanceRequest : Request
	{
		protected const string RequestNodeName = "BalanceRequest";
		protected const string BalanceNodeName = "Balance";
		protected const string RequestUrl = "https://api.libertyreserve.com/xml/balance.aspx";
		
		public List<BalanceOperation> Operations { get; set; }
				
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
	
	
	public class AccountNameRequest : Request
	{
		protected const string RequestNodeName = "AccountNameRequest";
		protected const string AccountNameNodeName = "AccountName";
		protected const string RequestUrl = "https://api.libertyreserve.com/xml/accountname.aspx";
		
		public List<AccountNameOperation> Operations { get; set; }
		
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

