
using CalculatorServiceok.Server.Services;
namespace CalculatorServiceok.Server.Services
{
	public interface ICalculatorService
	{
		double Add(IEnumerable<double> addends);
		double Subtract(double minuend, double subtrahend);
		double Multiply(IEnumerable<double> factors);
		(double Quotient, double Remainder) Divide(double dividend, double divisor);
		double SquareRoot(double number);
	}
}