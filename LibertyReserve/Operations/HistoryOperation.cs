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
		protected const int MaxPageSize = 20;
		internal const string PagerNodeName = "Pager";
		protected const string PageSizeNodeName = "PageSize";
		protected const string PageNumberNodeName = "PageNumber";
		protected const string PageCountNodeName = "PageCount";
		protected const string TotalCountNodeName = "TotalCount";
		
		public int? PageSize { get; set; }
		public int? PageNumber { get; set; }
		public int PageCount { get; set; }
		public int TotalCount { get; set; }
		
		public void Check()
		{
			if (PageSize != null && (PageSize <= 0 || PageSize > MaxPageSize))
			{
				string msg = String.Format(
					"'{0}' is not a valid page size value. It must be integer value from range (0..{1}].",
					PageSize, MaxPageSize);
				throw new LibertyReserveException(msg);
			}
			if (PageNumber != null && PageNumber <= 0)
				throw new LibertyReserveException("Page number must be greater than zero.");
		}
		
		public XElement ToXML()
		{
			return
				new XElement(PagerNodeName,
					PageSize == null ? null : new XElement(PageSizeNodeName, PageSize.Value),
					PageNumber == null ? null : new XElement(PageNumberNodeName, PageNumber.Value)
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
		
		protected static DateTime MinDate = new DateTime(2011, 1, 1);
		
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
		
		public void Check()
		{
			if (String.IsNullOrEmpty(AccountId))
				throw new LibertyReserveException("Account ID is missing.");
			if (String.IsNullOrEmpty(ReceiptId))
			{
				if (StartDate == null)
					throw new LibertyReserveException("Start date is missing.");
				else if (StartDate < MinDate)
					throw new LibertyReserveException(String.Format("History is not available before '{0}'.", MinDate));
				if (EndDate == null)
					throw new LibertyReserveException("End date is missing.");
			}
			if (StartDate != null && EndDate != null && StartDate > EndDate)
				throw new LibertyReserveException("End date must be greater than start date.");
			if (StartAmount != null && EndAmount != null && StartAmount > EndAmount)
				throw new LibertyReserveException("End amount must be greater than start amount.");
			if (Pager != null)
				Pager.Check();
		}
		
		public XElement ToXML()
		{
			return
				new XElement(OperationNodeName,
					Currency == null ? null : new XElement(CurrencyIdNodeName, LRConverter.ToString(Currency)),
					new XElement(AccountIdNodeName, AccountId),
					StartDate == null ? null : new XElement(StartDateNodeName, LRConverter.ToString(StartDate)),
					EndDate == null ? null : new XElement(EndDateNodeName, LRConverter.ToString(EndDate)),
					String.IsNullOrEmpty(CorrespondingAccountId) ? null : new XElement(CorrespondingAccountIdNodeName, CorrespondingAccountId),
					new XElement(DirectionNodeName, LRConverter.ToString(Direction)),
					String.IsNullOrEmpty(TransferId) ? null : new XElement(TransferIdNodeName, TransferId),
					String.IsNullOrEmpty(ReceiptId) ? null : new XElement(ReceiptIdNodeName, ReceiptId),
					String.IsNullOrEmpty(TransferType) ? null : new XElement(TransferTypeNodeName, TransferType),
					String.IsNullOrEmpty(Source) ? null : new XElement(SourceNodeName, Source),
					new XElement(AnonymousNodeName, LRConverter.ToString(Anonymity)),
					StartAmount == null ? null : new XElement(AmountFromNodeName, LRConverter.ToString(StartAmount)),
					EndAmount == null ? null : new XElement(AmountToNodeName, LRConverter.ToString(EndAmount)),
					Pager == null ? null : Pager.ToXML()
				);
		}
	}
}

