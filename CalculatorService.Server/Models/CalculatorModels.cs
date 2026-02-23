using System.Text.Json.Serialization;

namespace CalculatorService.Server.Models
{
    // --- MODELS FOR ADDITION ---
    // Represents the input for the sum operation
    public class AddRequest
    {
        public List<double> Addends { get; set; }
    }

    // Represents the output of the sum operation
    public class AddResponse
    {
        public double Sum { get; set; }
    }

    // --- MODELS FOR SUBTRACTION ---
    // Represents the input for the subtraction operation
    public class SubRequest
    {
        public double Minuend { get; set; }
        public double Subtrahend { get; set; }
    }

    // Represents the output of the subtraction operation
    public class SubResponse
    {
        public double Difference { get; set; }
    }

    // --- MODELS FOR MULTIPLICATION ---
    // Represents the input for the multiplication operation
    public class MultRequest
    {
        public List<double> Factors { get; set; }
    }

    // Represents the output of the multiplication operation
    public class MultResponse
    {
        public double Product { get; set; }
    }

    // --- MODELS FOR DIVISION ---
    // Represents the input for the division operation
    public class DivRequest
    {
        public double Dividend { get; set; }
        public double Divisor { get; set; }
    }

    // Represents the output of the division operation (including remainder)
    public class DivResponse
    {
        public double Quotient { get; set; }
        public double Remainder { get; set; }
    }

    // --- MODELS FOR SQUARE ROOT ---
    // Represents the input for the square root function
    public class SqrtRequest
    {
        public double Number { get; set; }
    }

    // Represents the output for the square root function
    public class SqrtResponse
    {
        public double Square { get; set; }
    }

    // --- MODELS FOR JOURNAL (History) ---
    // Used to query the journal by a specific Tracking-Id
    public class JournalQueryRequest
    {
        public string Id { get; set; }
    }

    // Represents a single entry in the operation history
    public class JournalEntry
    {
        public string Operation { get; set; }
        public string Calculation { get; set; }
        public DateTime Date { get; set; }
    }

    // Represents the list of operations returned when querying the journal
    public class JournalResponse
    {
        public List<JournalEntry> Operations { get; set; }
    }
}