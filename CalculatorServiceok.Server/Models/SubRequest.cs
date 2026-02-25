using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
	public class SubRequest
	{
		[Required]
		public double Minuend { get; set; }

		[Required]
		public double Subtrahend { get; set; }
	}
}