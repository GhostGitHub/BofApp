using BofApp.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BofApp.HealthChecks
{
	public class ObservationsHealthCheck : IHealthCheck
	{
		private readonly IExchangeRatesService _exchangeRatesService;

		public ObservationsHealthCheck(IExchangeRatesService exchangeRatesService)
		{
			_exchangeRatesService = exchangeRatesService;
		}

		public async Task<HealthCheckResult> CheckHealthAsync(
			HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			try
			{
				await _exchangeRatesService.GetExchangeRates();
				return HealthCheckResult.Healthy($"OK");
			}
			catch (Exception e)
			{
				return HealthCheckResult.Unhealthy("Failed to reach external API", e);
			}
		}
	}
}