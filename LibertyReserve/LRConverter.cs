using System;
using System.Globalization;

namespace Magnis.Web.Services.LibertyReserve
{
	public static class LRConverter
	{
		public static DateTime ToDateTime(string value)
		{
			return DateTime.ParseExact(value, "yyyy-dd-MM HH:mm:ss", CultureInfo.InvariantCulture);
		}
		
		public static string ToString(DateTime? timestamp)
		{
			return timestamp != null ? timestamp.Value.ToString("yyyy-dd-MM HH:mm:ss") : null;
		}
		
		public static double ToDouble(string value)
		{
			return Double.Parse(value, CultureInfo.InvariantCulture);
		}
		
		public static string ToString(double? amount)
		{
			return amount != null ? amount.Value.ToString("F4", CultureInfo.InvariantCulture) : null;
		}
		
		public static string ToString(Currency? currency)
		{
			switch (currency)
			{
			case null:
				return null;
			case Currency.EUR:
				return "LREUR";
			case Currency.USD:
				return "LRUSD";
			case Currency.Gold:
				return "LRGLD";
			default:
				throw new NotImplementedException();
			}
		}
		
		public static string ToString(TransactionDirection direction)
		{
			return direction.ToString().ToLower();
		}
		
		public static string ToString(Anonymity anonymity)
		{
			return anonymity.ToString().ToLower();
		}
		
		public static string ToString(HttpMethod? httpMethod)
		{
			return httpMethod != null ? httpMethod.ToString() : null;
		}
		
		public static Currency ToCurrency(string currency)
		{
			switch (currency.ToUpper())
			{
			case "LREUR":
				return Currency.EUR;
			case "LRUSD":
				return Currency.USD;
			case "LRGLD":
				return Currency.Gold;
			default:
				throw new ArgumentException();
			}
		}
	}
}

