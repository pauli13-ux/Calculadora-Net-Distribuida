using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
	public class MultRequest
	{
		[Required]
		[MinLength(2, ErrorMessage = "Se requieren al menos dos factores para multiplicar.")]
		public List<double> Factors { get; set; } = new();
	}
}