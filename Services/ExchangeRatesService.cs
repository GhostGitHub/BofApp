using BofApp.Controllers;
using System;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using static BofApp.Services.ExchangeRatesService;

namespace BofApp.Services
{
	public interface IExchangeRatesService
	{
		Task<List<ExchangeRatesResponse>> GetExchangeRates();
	}

	public class ExchangeRatesService : IExchangeRatesService
	{
		private readonly HttpClient _httpClient;
		private const string ApiUrl = "https://api.boffsaopendata.fi/referencerates/v2/api/V2";

		public ExchangeRatesService(IConfiguration config)
		{
			_httpClient = new HttpClient();
		}

		public async Task<List<ExchangeRatesResponse>> GetExchangeRates()
		{
			var response = await _httpClient.GetAsync(ApiUrl);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var exchangeRatesResponse = JsonSerializer.Deserialize<List<ExchangeRatesResponse>>(content, options);

			return exchangeRatesResponse;
		}

		public class ExchangeRatesResponse
		{
			[JsonPropertyName("ExchangeRates")]
			public List<ExchangeRatesDetail> ExchangeRates { get; set; }

			[JsonPropertyName("Currency")]
			public string Currency { get; set; }
		}

		public class ExchangeRatesData
		{
			[JsonPropertyName("ExchangeRates")]
			public ExchangeRatesDetail ExchangeRates { get; set; }

			[JsonPropertyName("Currency")]
			public string Currency { get; set; }

			[JsonPropertyName("CurrencyDenom")]
			public string CurrencyDenom { get; set; }
		}

		public class ExchangeRatesDetail
		{
			[JsonPropertyName("Value")]
			public string Value { get; set; }
		}
	}
}
