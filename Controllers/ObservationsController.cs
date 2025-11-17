using BofApp.Models;
using BofApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BofApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ObservationsController : ControllerBase
	{
		private readonly IExchangeRatesService _exchangeRatesService;

		private readonly HttpClient _httpClient;
		private const string BaseUrl = "https://api.boffsaopendata.fi/v4";
		private const string DataSet = "BOF_BKN1_PUBL";

		public ObservationsController(IExchangeRatesService exchangeRatesService)
		{
			_exchangeRatesService = exchangeRatesService;
			_httpClient = new HttpClient();
		}

		[HttpGet(Name = "GetObservations")]
		public async Task<ObservationResult> Get(string startPeriod, string endPeriod)
		{
			var queryParams = new List<string>();

			queryParams.Add($"startPeriod={startPeriod}");
			queryParams.Add($"endPeriod={endPeriod}");

			var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
			var url = $"{BaseUrl}/observations/{DataSet}{queryString}";

			var response = await _httpClient.GetAsync(url);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync();

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var observationsResponse = JsonSerializer.Deserialize<ObservationsResponse>(content, options);

			var ret = new ObservationResult() {
				Observations = new()
			};

			var exchangeRates = await _exchangeRatesService.GetExchangeRates();

			foreach (var observationItem in observationsResponse.Items)
			{
				var denomination = observationItem.Name.Split('.')[5];

				var pnOrPv = observationItem.Name.Split('.')[7];
				var amounts = new Dictionary<string, long>();
				var values = new Dictionary<string, long>();

				var cultureInfoFi = new CultureInfo("fi-FI");

				foreach (var observation in observationItem.Observations)
				{
					var newResult = ret.Observations.FirstOrDefault(o => o.Date == observation.Period);
					if (newResult == null)
					{
						newResult = new ObservationItemResult()
						{
							Date = observation.Period,
							Banknotes = new()
						};
						ret.Observations.Add(newResult);
					}

					var banknoteDetails = newResult.Banknotes.FirstOrDefault(bn => bn.Denomination == denomination);
					if (banknoteDetails == null)
					{
						banknoteDetails = new ObservationBankNoteDetail()
						{
							Denomination = denomination,
							Currencies = new()
						};
						newResult.Banknotes.Add(banknoteDetails);
					}

					switch (pnOrPv)
					{
						case "PN":
							banknoteDetails.Amount = observation.Value;
							break;
						case "PV":
							banknoteDetails.Value = observation.Value;
							foreach (var currency in exchangeRates)
							{
								// Use the first (latest?) currency exchange rate for the conversion
								decimal currencyRate = decimal.Parse(currency.ExchangeRates.First().Value, cultureInfoFi);
								banknoteDetails.Currencies[currency.Currency] = observation.Value * currencyRate;
							}
							break;
						default:
							// Skip unknown
							break;
					}
				}
			}

			return ret;
		}
	}
}
