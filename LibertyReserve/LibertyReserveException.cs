using System;
using System.Runtime.Serialization;

namespace Magnis.Web.Services.LibertyReserve
{
	[Serializable]
	public class LibertyReserveException : Exception
	{
		public int ErrorCode { get; set; }
		public string Description { get; set; }
		
		public LibertyReserveException()
			: base()
		{
			
		}
		
		public LibertyReserveException(int errorCode, string message, string description)
			:  this(errorCode, message, description, null)
		{
		}
		
		public LibertyReserveException(int errorCode, string message, string description, Exception innerException)
			: base(message, innerException)
		{
			ErrorCode = errorCode;
			Description = description;
		}
		
		protected LibertyReserveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

