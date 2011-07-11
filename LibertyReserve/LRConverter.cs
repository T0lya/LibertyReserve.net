using System;
using System.Globalization;

namespace Magnis.Web.Services.LibertyReserve
{
	public static class LRConverter
	{
		public static DateTime ToDateTime(string value)
		{
			return DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		}
		
		public static string ToString(Currency currency)
		{
			switch (currency)
			{
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
		
		public static Currency FromString(string currency)
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

