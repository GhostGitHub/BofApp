using System.Text.Json.Serialization;

namespace BofApp.Models
{
	// Objects for returning results from our API

	public class ObservationResult
	{
		public required List<ObservationItemResult> Observations { get; set; }
	}

	public class ObservationItemResult
	{
		public DateOnly Date { get; set; }
		public required List<ObservationBankNoteDetail> Banknotes { get; set; }
	}

	public class ObservationBankNoteDetail
	{
		public required string Denomination { get; set; }

		/// <summary>
		/// Bank note code -> total value of bank notes
		/// </summary>
		public long Value { get; set; }

		/// <summary>
		/// Bank note code -> amount of bank notes
		/// </summary>
		public long Amount { get; set; }

		/// <summary>
		/// Value converted to available currencies
		/// </summary>
		public required Dictionary<string, decimal> Currencies { get; set; }
	}


	// Objects used to parse external API responses

	public class ObservationsResponse
	{
		[JsonPropertyName("items")]
		public required List<ObservationItem> Items { get; set; }
	}

	public class ObservationItem
	{
		[JsonPropertyName("name")]
		public required string Name { get; set; }

		[JsonPropertyName("observations")]
		public required List<Observation> Observations { get; set; }
	}

	public class Observation
	{
		[JsonPropertyName("period")]
		public DateOnly Period { get; set; }

		[JsonPropertyName("value")]
		public long Value { get; set; }
	}
}
