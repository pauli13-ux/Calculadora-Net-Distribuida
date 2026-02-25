using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
	public class SqrtRequest
	{
		[Required]
		public double Number { get; set; }
	}
}