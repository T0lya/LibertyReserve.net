using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Magnis.Web.Services.LibertyReserve
{
	public class SCIResponse
	{
		protected const string MerchantAccountFieldName = "lr_paidto";
		protected const string BuyerAccountFieldName = "lr_paidby";
		protected const string AmountFieldName = "lr_amnt";
		protected const string FeeFieldName = "lr_fee_amnt";
		protected const string CurrencyFieldName = "lr_currency";
		protected const string TransactionIdFieldName = "lr_transfer";
		protected const string StoreFieldName = "lr_store";
		protected const string TransactionTimestampFieldName = "lr_timestamp";
		protected const string MerchantRefFieldName = "lr_merchant_ref";
		protected const string EncryptedFieldName = "lr_encrypted";
		protected const string Encrypted2FieldName = "lr_encrypted2";
				
		public string MerchantAccount { get; set; }
		public string BuyerAccount { get; set; }
		public string Store { get; set; }
		public double Amount { get; set; }
		protected string AmountValue { get; set; }
		public double Fee { get; set; }
		public Currency Currency { get; set; }
		private string CurrencyValue { get; set; }
		public string TransactionId { get; set; }
		public DateTime TransactionTimestamp { get; set; }
		public string MerchantRef { get; set; }
		public string Encrypted { get; set; }
		public string Encrypted2 { get; set; }
		public Dictionary<string, string> BaggageFields { get; set; }
				
		public static SCIResponse Parse(NameValueCollection formData)
		{
			var response = new SCIResponse
			{
				BaggageFields = new Dictionary<string, string>(),
			};
			string key;
			for (int i = 0; i < formData.AllKeys.Length; i++)
			{
				key = formData.AllKeys[i];
				if (key.Equals(MerchantAccountFieldName, StringComparison.OrdinalIgnoreCase))
					response.MerchantAccount = formData[MerchantAccountFieldName];
				if (key.Equals(BuyerAccountFieldName, StringComparison.OrdinalIgnoreCase))
					response.BuyerAccount = formData[BuyerAccountFieldName];
				else if (key.Equals(StoreFieldName, StringComparison.OrdinalIgnoreCase))
					response.Store = formData[StoreFieldName];
				else if (key.Equals(AmountFieldName, StringComparison.OrdinalIgnoreCase))
				{
					response.Amount = LRConverter.ToDouble(formData[AmountFieldName]);
					response.AmountValue = formData[AmountFieldName];
				}
				else if (key.Equals(FeeFieldName, StringComparison.OrdinalIgnoreCase))
					response.Fee = LRConverter.ToDouble(formData[FeeFieldName]);
				else if (key.Equals(CurrencyFieldName, StringComparison.OrdinalIgnoreCase))
				{
					response.Currency = LRConverter.ToCurrency(formData[CurrencyFieldName]);
					response.CurrencyValue = formData[CurrencyFieldName];
				}
				else if (key.Equals(TransactionIdFieldName, StringComparison.OrdinalIgnoreCase))
					response.TransactionId = formData[TransactionIdFieldName];
				else if (key.Equals(TransactionTimestampFieldName, StringComparison.OrdinalIgnoreCase))
					response.TransactionTimestamp = LRConverter.ToDateTime(formData[TransactionTimestampFieldName]);
				else if (key.Equals(MerchantRefFieldName, StringComparison.OrdinalIgnoreCase))
					response.MerchantRef = formData[MerchantRefFieldName];
				else if (key.Equals(EncryptedFieldName, StringComparison.OrdinalIgnoreCase))
					response.Encrypted = formData[EncryptedFieldName];
				else if (key.Equals(Encrypted2FieldName, StringComparison.OrdinalIgnoreCase))
					response.Encrypted2 = formData[Encrypted2FieldName];
				else
					response.BaggageFields[key] = formData[key];
			}
			
			return response;
		}
		
		public bool CanValidate()
		{
			return !String.IsNullOrEmpty(Encrypted2);
		}
		
		public bool Validate(string storeSecurityKey)
		{
			var concatenatedBaggageFields = new StringBuilder();
			if (concatenatedBaggageFields.Length > 0)
			{
				foreach (KeyValuePair<string, string> pair in BaggageFields)
					concatenatedBaggageFields.AppendFormat("{0}={1};", pair.Key, pair.Value);
				concatenatedBaggageFields.Remove(concatenatedBaggageFields.Length - 1, 1);
			}
			string data = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}",
				MerchantAccount,
				BuyerAccount,
				Store,
				AmountValue,
				TransactionId,
				MerchantRef,
				concatenatedBaggageFields,
				CurrencyValue,
				storeSecurityKey);
			string hash = ComputeHash(data);
			
			return Encrypted2.Equals(hash);
		}
		
		private string ComputeHash(string data)
		{
			using (var sha = new SHA256Managed())
			{
				byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
				string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
				
				return hash;
			}
		}
	}

}

