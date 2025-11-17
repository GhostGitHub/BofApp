using BofApp.Models;
using System.Text.Json;

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
			if (exchangeRatesResponse == null)
			{
				throw new Exception("Invalid exchange rates response");
			}

			return exchangeRatesResponse;
		}
	}
}
