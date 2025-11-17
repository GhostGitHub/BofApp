using System.Text.Json.Serialization;

namespace BofApp.Models
{
	public class ExchangeRatesResponse
	{
		[JsonPropertyName("ExchangeRates")]
		public required List<ExchangeRatesDetail> ExchangeRates { get; set; }

		[JsonPropertyName("Currency")]
		public required string Currency { get; set; }
	}

	public class ExchangeRatesData
	{
		[JsonPropertyName("ExchangeRates")]
		public required ExchangeRatesDetail ExchangeRates { get; set; }

		[JsonPropertyName("Currency")]
		public required string Currency { get; set; }

		[JsonPropertyName("CurrencyDenom")]
		public required string CurrencyDenom { get; set; }
	}

	public class ExchangeRatesDetail
	{
		[JsonPropertyName("Value")]
		public required string Value { get; set; }
	}
}
