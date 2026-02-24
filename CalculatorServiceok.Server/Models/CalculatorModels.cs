using System.ComponentModel.DataAnnotations;

namespace CalculatorServiceok.Server.Models
{
    public class AddRequest 
    {
        [MinLength(2)]
        public IEnumerable<double> Addends { get; set; } = new();
    }
    public class SubRequest { public double Minuend { get; set; } public double Subtrahend { get; set; } }
    public class MultRequest { public List<double> Factors { get; set; } = new(); }
    public class DivRequest { public double Dividend { get; set; } public double Divisor { get; set; } }
    public class SqrtRequest { public double Number { get; set; } }

    public class JournalEntry
    {
        public string Operation { get; set; } = "";

        public string Calculation { get; set; } = "";
        public DateTime Date { get; set; }
    }
}