using NUnit.Framework;
using CalculatorServiceok.Server.Services;
using System.Collections.Generic;

namespace CalculatorServices.Tests
{
	[TestFixture]
	public class CalculatorServiceTests
	{
		private readonly CalculatorService _service;

		public CalculatorServiceTests()
		{
			// We instantiate the real service to test it
			_service = new CalculatorService();
		}

		[Test]
		public void Add_ShouldReturnCorrectSum()
		{
			// Arrange
			var numbers = new List<double> { 5, 10, 15 };

			// Act
			var result = _service.Add(numbers);

			// Assert
			Assert.That(result, Is.EqualTo((double)30.0));
		}

		[Test]
		public void Divide_ShouldReturnQuotientAndRemainder()
		{
			// Arrange
			double dividend = 10;
			double divisor = 3;

			// FIXME ENGLISH Act - We use the tuple returned by your CalculatorService
			var result = _service.Divide(dividend, divisor);

			// Assert - We compare decimals (double)
			Assert.That(result.Quotient, Is.EqualTo(3.3333333333333335).Within(0.0000000001));
			Assert.That(result.Remainder, Is.EqualTo(1.0));
		}

		[TestCase(4, 2)]  // En NUnit, InlineData se llama TestCase
		[TestCase(9, 3)]
		[TestCase(25, 5)]
		public void SquareRoot_ShouldCalculateCorrectly(double input, double expected)
		{
			var result = _service.SquareRoot(input);
			Assert.That(result, Is.EqualTo(expected));
		}
	}
}