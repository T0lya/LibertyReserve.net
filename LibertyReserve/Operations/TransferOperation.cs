using System;
using System.Xml.Linq;

namespace Magnis.Web.Services.LibertyReserve
{
	public class TransferOperation
	{
		protected const string OperationNodeName = "Transfer";
		protected const string TransferIdNodeName = "TransferId";
		protected const string TransferTypeNodeName = "TransferType";
		protected const string PayerNodeName = "Payer";
		protected const string PayeeNodeName = "Payee";
		protected const string CurrencyIdNodeName = "CurrencyId";
		protected const string AmountNodeName = "Amount";
		protected const string MemoNodeName = "Memo";
		protected const string AnonymousNodeName = "Anonymous";
		
		private const string TransferType = "transfer";
		
		public string TransferId { get; set; }
		public string Payer { get; set; }
		public string Payee { get; set; }
		public Currency Currency { get; set; }
		public double Amount { get; set; }
		public string Description { get; set; }
		public bool Anonymous { get; set; }
		
		public void Check()
		{
			if (String.IsNullOrEmpty(Payer))
				throw new LibertyReserveException("Payer is missing.");
			if (String.IsNullOrEmpty(Payee))
				throw new LibertyReserveException("Payee is missing.");
			if (Amount <= 0.0)
				throw new LibertyReserveException("Invalid amount value.");
		}
		
		public XElement ToXML()
		{
			return
				new XElement(OperationNodeName,
					new XElement(TransferIdNodeName, TransferId),
					new XElement(TransferTypeNodeName, TransferType),
					new XElement(PayerNodeName, Payer),
					new XElement(PayeeNodeName, Payee),
					new XElement(CurrencyIdNodeName, LRConverter.ToString(Currency)),
					new XElement(AmountNodeName, LRConverter.ToString(Amount)),
					new XElement(MemoNodeName, Description),
					new XElement(AnonymousNodeName, Anonymous)
				);
		}
	}
}

