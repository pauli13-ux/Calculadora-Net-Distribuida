using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
	public class MultRequest
	{
		[Required]
		[MinLength(2, ErrorMessage = "At least two factors are required for multiplication.")]
		public List<double> Factors { get; set; } = new();
	}
}