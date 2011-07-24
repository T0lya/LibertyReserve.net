using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;

namespace Magnis.Web.Services.LibertyReserve
{
	public enum HttpMethod
	{
		GET,
		POST,
		LINK
	}
		
	public class SCIRequest
	{
		protected const string SciUrl = "https://sci.libertyreserve.com/";
		protected const string MerchantAccountFieldName = "lr_acc";
		protected const string BuyerAccountFieldName = "lr_acc_from";
		protected const string StoreFieldName = "lr_store";
		protected const string AmountFieldName = "lr_amnt";
		protected const string CurrencyFieldName = "lr_currency";
		protected const string CommentsFieldName = "lr_comments";
		protected const string MerchantRefFieldName = "lr_merchant_ref";
		protected const string SuccessUrlFieldName = "lr_success_url";
		protected const string SuccessUrlMethodFieldName = "lr_success_url_method";
		protected const string FailUrlFieldName = "lr_fail_url";
		protected const string FailUrlMethodFieldName = "lr_fail_url_method";
		protected const string StatusUrlFieldName = "lr_status_url";
		protected const string StatusUrlMethodFieldName = "lr_status_url_method";
		
		protected const int MaxStoreNameLength = 50;
		protected const int MaxCommentsLength = 100;
		protected const int MaxMerchantRefLength = 20;
		protected const int MaxUrlLength = 100;
		protected const int MaxBaggageFieldLength = 50;
		
		public string MerchantAccount { get; set; }
		public string BuyerAccount { get; set; }
		public string Store { get; set; }
		public double? Amount { get; set; }
		public Currency? Currency { get; set; }
		public string Comments { get; set; }
		public string MerchantRef { get; set; }
		public Uri SuccessUrl { get; set; }
		public HttpMethod? SuccessUrlMethod { get; set; }
		public Uri FailUrl { get; set; }
		public HttpMethod? FailUrlMethod { get; set; }
		public Uri StatusUrl { get; set; }
		public HttpMethod? StatusUrlMethod { get; set; }
		public Dictionary<string, string> BaggageFields { get; set; }
		
		public void Check()
		{
			if (String.IsNullOrEmpty(MerchantAccount))
				throw new LibertyReserveException("Merchant account is missing.");
			if (Amount != null && Amount <= 0)
				throw new LibertyReserveException("Invalid amount value.");
			if (Store != null && HttpUtility.HtmlEncode(Store).Length > MaxStoreNameLength)
			{
				throw new LibertyReserveException(String.Format(
					"Store name must not be longer than {0} characters.", MaxStoreNameLength));
			}
			if (Comments != null && HttpUtility.HtmlEncode(Comments).Length > MaxCommentsLength)
			{
				throw new LibertyReserveException(String.Format(
					"Comments must not be longer than {0} characters.", MaxCommentsLength));
			}
			if (MerchantRef != null && HttpUtility.HtmlEncode(MerchantRef).Length > MaxMerchantRefLength)
			{
				throw new LibertyReserveException(String.Format(
					"Merchant ref must not be longer than {0} characters.", MaxCommentsLength));
			}
			if (SuccessUrl != null && SuccessUrl.AbsoluteUri.Length > MaxUrlLength)
			{
				throw new LibertyReserveException(String.Format(
					"Success url must not be longer than {0} characters.", MaxUrlLength));
			}
			if (FailUrl != null && FailUrl.AbsoluteUri.Length > MaxUrlLength)
			{
				throw new LibertyReserveException(String.Format(
					"Fail url must not be longer than {0} characters.", MaxUrlLength));
			}
			if (StatusUrl != null && StatusUrl.AbsoluteUri.Length > MaxUrlLength)
			{
				throw new LibertyReserveException(String.Format(
					"Status url must not be longer than {0} characters.", MaxUrlLength));
			}
			if (StatusUrlMethod != null && 
				StatusUrlMethod != HttpMethod.GET && 
				StatusUrlMethod != HttpMethod.POST)
			{
				throw new LibertyReserveException(String.Format(
					"Invalid status url method: '{0}'.", StatusUrlMethod));
			}
			if (BaggageFields != null)
			{
				if (BaggageFields.Any(pair => 
					HttpUtility.HtmlEncode(pair.Key).Length > MaxBaggageFieldLength ||
					HttpUtility.HtmlEncode(pair.Value).Length > MaxBaggageFieldLength))
				{
					throw new LibertyReserveException(String.Format(
						"Baggage field must not be longer than {0} characters.", MaxBaggageFieldLength));
				}
			}
		}
		
		public string GeneratePaymentForm(string formContentHtml)
		{
			XElement html = 
				new XElement("form",
					new XAttribute("method", "POST"),
					new XAttribute("action", SciUrl),
					GenerateHiddenField(MerchantAccountFieldName, MerchantAccount),
					String.IsNullOrEmpty(BuyerAccount) ? null : GenerateHiddenField(BuyerAccountFieldName, BuyerAccount),
					String.IsNullOrEmpty(Store) ? null : GenerateHiddenField(StoreFieldName, Store),
					Amount == null ? null : GenerateHiddenField(AmountFieldName, LRConverter.ToString(Amount)),
					Currency == null ? null : GenerateHiddenField(CurrencyFieldName, LRConverter.ToString(Currency)),
					String.IsNullOrEmpty(Comments) ? null : GenerateHiddenField(CommentsFieldName, Comments),
					String.IsNullOrEmpty(MerchantRef) ? null : GenerateHiddenField(MerchantRefFieldName, MerchantRef),
					SuccessUrl == null ? null : GenerateHiddenField(SuccessUrlFieldName, SuccessUrl.AbsoluteUri),
					SuccessUrl == null || SuccessUrlMethod == null ? null : GenerateHiddenField(SuccessUrlMethodFieldName, LRConverter.ToString(SuccessUrlMethod)),
					FailUrl == null ? null : GenerateHiddenField(FailUrlFieldName, FailUrl.AbsoluteUri),
					FailUrl == null || FailUrlMethod == null ? null : GenerateHiddenField(FailUrlMethodFieldName, LRConverter.ToString(FailUrlMethod)),
					StatusUrl == null ? null : GenerateHiddenField(StatusUrlFieldName, StatusUrl.AbsoluteUri),
					StatusUrl == null || StatusUrlMethod == null ? null : GenerateHiddenField(StatusUrlMethodFieldName, LRConverter.ToString(StatusUrlMethod)),
					BaggageFields == null ? null : BaggageFields.Select(pair => GenerateHiddenField(pair.Key, pair.Value)),
					String.IsNullOrEmpty(formContentHtml) ? null : XElement.Parse(formContentHtml)
					);
			
			return html.ToString();
		}
		
		protected XElement GenerateHiddenField(string name, string value)
		{
			return
				new XElement("input", 
					new XAttribute("type", "hidden"),
					new XAttribute("name", HttpUtility.HtmlEncode(name)),
					new XAttribute("value", HttpUtility.HtmlEncode(value))
				);
		}
	}
}

