using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class ApiError
	{
		protected const string CodeNodeName = "Code";
		protected const string TextNodeName = "Text";
		protected const string DescriptionNodeName = "Description";
		
		public int Code { get; set; }
		public string Text { get; set; }
		public string Description { get; set; }
		
		public static ApiError Parse(XElement xml)
		{
			var error = new ApiError
			{
				Code = Int32.Parse(xml.Element(CodeNodeName).Value),
				Text = xml.Element(TextNodeName).Value.Trim(),
				Description = xml.Element(DescriptionNodeName).Value.Trim(),
			};
			
			return error;
		}
	}
	
	
	public abstract class Response
	{
		protected const string RequestIdAttributeName = "id";
		protected const string ResponseDateAttributeName = "date";
		protected const string ErrorNodeName = "Error";
		
		public string ResponseText { get; set; }
		public string RequestId { get; set; }
		public DateTime Timestamp { get; set; }
		public List<ApiError> Errors { get; protected set; }
		
		protected void ParseHeader(XElement xml)
		{
			ResponseText = xml.ToString();
			RequestId = xml.Attribute(RequestIdAttributeName).Value;
			Timestamp = LRConverter.ToDateTime(xml.Attribute(ResponseDateAttributeName).Value);
		}
		
		protected void ParseErrors(XElement xml)
		{
			Errors = new List<ApiError>();
			foreach (XElement node in xml.Elements(ErrorNodeName))
			{
				Errors.Add(ApiError.Parse(node));
			}
		}
	}
}
