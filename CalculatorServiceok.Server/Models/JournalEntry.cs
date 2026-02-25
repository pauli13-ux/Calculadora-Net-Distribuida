using System;

namespace CalculatorServiceok.Server.Models
{
	public class JournalEntry
	{
		public string Operation { get; set; } = string.Empty;
		public string Calculation { get; set; } = string.Empty;
		public DateTime Date { get; set; } = DateTime.Now;
	}
}