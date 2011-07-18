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
	
	
	public class Pager
	{
		internal const string PagerNodeName = "Pager";
		protected const string PageSizeNodeName = "PageSize";
		protected const string PageNumberNodeName = "PageNumber";
		protected const string PageCountNodeName = "PageCount";
		protected const string TotalCountNodeName = "TotalCount";
		
		public int? PageSize { get; set; }
		public int? PageNumber { get; set; }
		public int PageCount { get; set; }
		public int TotalCount { get; set; }
		
		public XElement ToXML()
		{
			return
				new XElement(PagerNodeName,
					new XElement(PageSizeNodeName, PageSize != null ? PageSize.ToString() : String.Empty),
					new XElement(PageNumberNodeName, PageNumber != null ? PageNumber.ToString() : String.Empty)
				);
		}
		
		public static Pager Parse(XElement xml)
		{
			var pager = new Pager
			{
				PageSize = Int32.Parse(xml.Element(PageSizeNodeName).Value),
				PageNumber = Int32.Parse(xml.Element(PageNumberNodeName).Value),
				PageCount = Int32.Parse(xml.Element(PageCountNodeName).Value),
				TotalCount = Int32.Parse(xml.Element(TotalCountNodeName).Value),
			};
			
			return pager;
		}
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
		
		public Currency? Currency { get; set; }
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
		public Pager Pager { get; set; }
		
		public HistoryOperation()
		{
			Anonymity = Anonymity.Any;
			Direction = TransactionDirection.Any;
		}
		
		public XElement ToXML()
		{
			return
				new XElement(OperationNodeName,
					new XElement(CurrencyIdNodeName, Currency != null ? LRConverter.ToString(Currency.Value) : String.Empty),
					new XElement(AccountIdNodeName, AccountId),
					new XElement(StartDateNodeName, StartDate != null ? LRConverter.ToString(StartDate.Value) : String.Empty),
					new XElement(EndDateNodeName, EndDate != null ? LRConverter.ToString(EndDate.Value) : String.Empty),
					new XElement(CorrespondingAccountIdNodeName, CorrespondingAccountId ?? String.Empty),
					new XElement(DirectionNodeName, LRConverter.ToString(Direction)),
					new XElement(TransferIdNodeName, TransferId ?? String.Empty),
					new XElement(ReceiptIdNodeName, ReceiptId ?? String.Empty),
					new XElement(TransferTypeNodeName, TransferType ?? String.Empty),
					new XElement(SourceNodeName, Source ?? String.Empty),
					new XElement(AnonymousNodeName, LRConverter.ToString(Anonymity)),
					new XElement(AmountFromNodeName, StartAmount != null ? LRConverter.ToString(StartAmount.Value) : String.Empty),
					new XElement(AmountToNodeName, EndAmount != null ? LRConverter.ToString(EndAmount.Value) : String.Empty),
					Pager != null ? Pager.ToXML() : new XElement(Pager.PagerNodeName)
				);
		}
	}
}

