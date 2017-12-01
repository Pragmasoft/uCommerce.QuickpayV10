using System;

namespace Pragmasoft.QuickpayV10.Extensions.Extensions
{
	public static class DecimalExtensions
	{
		public static int ToCents(this Decimal amount)
		{
			return Convert.ToInt32(Decimal.Round(amount, 2) * new Decimal(100));
		}

		public static int ToCents(this Decimal? amount)
		{
			return ToCents(amount.GetValueOrDefault(new Decimal(0)));
		}
	}
}
