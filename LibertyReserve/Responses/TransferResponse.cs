using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Magnis.Web.Services.LibertyReserve
{
	public class Transfer
	{
		protected const string TransferIdNodeName = "TransferId";
		protected const string TransferTypeNodeName = "TransferType";
		protected const string PayerNodeName = "Payer";
		protected const string PayeeNodeName = "Payee";
		protected const string CurrencyNodeName = "CurrencyId";
		protected const string AmountNodeName = "Amount";
		protected const string DescriptionNodeName = "Memo";
		protected const string AnonymousNodeName = "Anonymous";
		protected const string SourceNodeName = "Source";
		
		public string TransferId { get; set; }
		public string TransferType { get; set; }
		public string Payer { get; set; }
		public string Payee { get; set; }
		public Currency Currency { get; set; }
		public double Amount { get; set; }
		public string Description { get; set; }
		public bool Anonymous { get; set; }
		public string Source { get; set; }
		
		public static Transfer Parse(XElement xml)
		{
			var transfer = new Transfer
			{
				TransferId = xml.Element(TransferIdNodeName).Value.Trim(),
				TransferType = xml.Element(TransferTypeNodeName).Value.Trim(),
				Payer = xml.Element(PayerNodeName).Value.Trim(),
				Payee = xml.Element(PayeeNodeName).Value.Trim(),
				Currency = LRConverter.ToCurrency(xml.Element(CurrencyNodeName).Value.Trim()),
				Amount = LRConverter.ToDouble(xml.Element(AmountNodeName).Value),
				Description = xml.Element(DescriptionNodeName).Value.Trim(),
				Anonymous = Boolean.Parse(xml.Element(AnonymousNodeName).Value),
				Source = xml.Element(SourceNodeName).Value.Trim(),
			};
			
			return transfer;
		}
	}
	
	
	public class Receipt
	{
		protected const string ReceiptIdNodeName = "ReceiptId";
		protected const string DateNodeName = "Date";
		protected const string PayerNodeName = "PayerName";
		protected const string PayeeNodeName = "PayeeName";
		protected const string AmountNodeName = "Amount";
		protected const string FeeNodeName = "Fee";
		protected const string ClosingBalanceNodeName = "ClosingBalance";
		protected const string TransferNodeName = "Transfer";
		
		public string ReceiptId { get; set; }
		public DateTime Timestamp { get; set; }
		public string PayerName { get; set; }
		public string PayeeName { get; set; }
		public double Amount { get; set; }
		public double Fee { get; set; }
		public double ClosingBalance { get; set; }
		public Transfer Transfer { get; set; }
		
		public static Receipt Parse(XElement xml)
		{
			var receipt = new Receipt
			{
				ReceiptId = xml.Element(ReceiptIdNodeName).Value.Trim(),
				Timestamp = LRConverter.ToDateTime(xml.Element(DateNodeName).Value.Trim()),
				PayerName = xml.Element(PayerNodeName).Value.Trim(),
				PayeeName = xml.Element(PayeeNodeName).Value.Trim(),
				Amount = LRConverter.ToDouble(xml.Element(AmountNodeName).Value),
				Fee = LRConverter.ToDouble(xml.Element(FeeNodeName).Value),
				ClosingBalance = LRConverter.ToDouble(xml.Element(ClosingBalanceNodeName).Value),
				Transfer = Transfer.Parse(xml.Element(TransferNodeName)),
			};
			
			return receipt;
		}
	}
	
	
	public class TransferResponse : Response
	{
		protected const string ReceiptNodeName = "Receipt";
		
		public List<Receipt> Receipts { get; set; }
		
		public TransferResponse()
		{
			Receipts = new List<Receipt>();
		}
		
		public static TransferResponse Parse(string responseText)
		{
			try
			{
				XElement xml = XElement.Parse(responseText);
				var response = new TransferResponse();
				response.ParseHeader(xml);
				foreach (XElement node in xml.Elements(ReceiptNodeName))
				{
					Receipt r = Receipt.Parse(node);
					response.Receipts.Add(r);
				}
				response.ParseErrors(xml);
				
				return response;
			}
			catch (Exception e)
			{
				throw new LibertyReserveException("Transfer response format is invalid.", responseText, e);
			}
		}
	}
}

