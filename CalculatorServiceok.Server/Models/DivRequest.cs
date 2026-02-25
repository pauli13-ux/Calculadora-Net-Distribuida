using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
	public class DivRequest
	{
		[Required]
		public double Dividend { get; set; }

		[Required]
		// Note: The divisor != 0 validation is done in the Controller
		public double Divisor { get; set; }
	}
}