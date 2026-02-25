namespace CalculatorServiceok.Server.Services
{
	public class CalculatorService : ICalculatorService
	{
		public double Add(IEnumerable<double> addends) => addends.Sum();

		public double Subtract(double minuend, double subtrahend) => minuend - subtrahend;

		public double Multiply(IEnumerable<double> factors) => factors.Aggregate(1.0, (a, b) => a * b);

		public (double Quotient, double Remainder) Divide(double dividend, double divisor)
		{
			// La lógica de negocio vive aquí
			return (dividend / divisor, dividend % divisor);
		}

		public double SquareRoot(double number) => Math.Sqrt(number);
	}
}