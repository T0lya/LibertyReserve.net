using System;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public enum TransactionDirection
	{
		Any,
		Incoming,
		Outgoing
	}
	
	
	public enum Anonymity
	{
		Any,
		Yes,
		No
	}
	
	
	public class HistoryOperation
	{
		protected const string OperationNodeName = "History";
		protected const string CurrencyIdNodeName = "CurrencyId";
		protected const string AccountIdNodeName = "AccountId";
		protected const string StartDateNodeName = "From";
		protected const string EndDateNodeName = "Till";
		protected const string CorrespondingAccountIdNodeName = "CorrespondingAccountId";
		protected const string DirectionNodeName = "Direction";
		protected const string TransferIdNodeName = "TransferId";
		protected const string ReceiptIdNodeName = "ReceiptId";
		protected const string TransferTypeNodeName = "TransferType";
		protected const string SourceNodeName = "Source";
		protected const string AnonymousNodeName = "Anonymous";
		protected const string AmountFromNodeName = "AmountFrom";
		protected const string AmountToNodeName = "AmountTo";
		
		public Currency Currency { get; set; }
		public string AccountId { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string CorrespondingAccountId { get; set; }
		public TransactionDirection Direction { get; set; }
		public string TransferId { get; set; }
		public string ReceiptId { get; set; }
		public string TransferType { get; set; }
		public string Source { get; set; }
		public Anonymity Anonymity { get; set; }
		public double? StartAmount { get; set; }
		public double? EndAmount { get; set; }
		
		public HistoryOperation()
		{
			Anonymity = Anonymity.Any;
			Direction = TransactionDirection.Any;
		}
		
		public XElement ToXML()
		{
			return
				new XElement(OperationNodeName,
					new XElement(CurrencyIdNodeName, LRConverter.ToString(Currency)),
					new XElement(AccountIdNodeName, AccountId),
					new XElement(StartDateNodeName, StartDate != null ? LRConverter.ToString(StartDate.Value) : String.Empty),
					new XElement(EndDateNodeName, EndDate != null ? LRConverter.ToString(EndDate.Value) : String.Empty),
					new XElement(CorrespondingAccountIdNodeName, CorrespondingAccountId ?? String.Empty),
					new XElement(DirectionNodeName, LRConverter.ToString(Direction)),
					new XElement(TransferIdNodeName, TransferId ?? String.Empty),
					new XElement(ReceiptIdNodeName, ReceiptId ?? String.Empty),
					new XElement(TransferTypeNodeName, TransferType ?? String.Empty),
					new XElement(SourceNodeName, Source ?? String.Empty),
					new XElement(AnonymousNodeName, LRConverter.ToString(Anonymity) )
				);
		}
	}
}

