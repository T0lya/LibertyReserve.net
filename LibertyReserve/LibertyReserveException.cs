using System;
using System.Runtime.Serialization;

namespace Magnis.Web.Services.LibertyReserve
{
	[Serializable]
	public class LibertyReserveException : Exception
	{
		public string ResponseText { get; set; }
		
		public LibertyReserveException()
			: base()
		{
		}
		
		public LibertyReserveException(string message)
			:  base(message)
		{
		}
		
		public LibertyReserveException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
		
		public LibertyReserveException(string message, string responseText, Exception innerException)
			: base(message, innerException)
		{
			ResponseText = responseText;
		}
		
		protected LibertyReserveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

