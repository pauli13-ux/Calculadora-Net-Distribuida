using Microsoft.AspNetCore.Mvc;
using CalculatorService.Server.Models;
using CalculatorService.Server.Services;

namespace CalculatorService.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly JournalService _journalService;

        public CalculatorController(JournalService journalService)
        {
            _journalService = journalService;
        }

        [HttpGet("test")]
        public IEnumerable<string> Test()
        {
            return new List<string>() {  "esto es la confirmacion de la prueba" };
        }

        // POST /calculator/add
        [HttpPost("add")]
        public IActionResult Add([FromBody] AddRequest request)
        {
            double sum = 0;
            string calculation = "";

            // Business logic: Sum all elements in the list
            for (int i = 0; i < request.Addends.Count; i++)
            {
                sum += request.Addends[i];
                calculation += request.Addends[i] + (i < request.Addends.Count - 1 ? " + " : "");
            }
            calculation += $" = {sum}";

            // Check for Tracking-Id header and save to journal
            if (Request.Headers.TryGetValue("X-Evi-Tracking-Id", out var trackingId))
            {
                _journalService.AddEntry(trackingId, "Sum", calculation);
            }

            return Ok(new AddResponse { Sum = sum });
        }

        // POST /calculator/sub
        [HttpPost("sub")]
        public IActionResult Subtract([FromBody] SubRequest request)
        {
            double difference = request.Minuend - request.Subtrahend;
            string calculation = $"{request.Minuend} - {request.Subtrahend} = {difference}";

            if (Request.Headers.TryGetValue("X-Evi-Tracking-Id", out var trackingId))
            {
                _journalService.AddEntry(trackingId, "Sub", calculation);
            }

            return Ok(new SubResponse { Difference = difference });
        }

        // POST /calculator/mult
        [HttpPost("mult")]
        public IActionResult Multiply([FromBody] MultRequest request)
        {
            double product = 1;
            string calculation = "";

            for (int i = 0; i < request.Factors.Count; i++)
            {
                product *= request.Factors[i];
                calculation += request.Factors[i] + (i < request.Factors.Count - 1 ? " * " : "");
            }
            calculation += $" = {product}";

            if (Request.Headers.TryGetValue("X-Evi-Tracking-Id", out var trackingId))
            {
                _journalService.AddEntry(trackingId, "Mul", calculation);
            }

            return Ok(new MultResponse { Product = product });
        }

        // POST /calculator/div
        [HttpPost("div")]
        public IActionResult Divide([FromBody] DivRequest request)
        {
            if (request.Divisor == 0)
            {
                return BadRequest(new { ErrorMessage = "Divisor cannot be zero" });
            }

            double quotient = Math.Floor(request.Dividend / request.Divisor);
            double remainder = request.Dividend % request.Divisor;
            string calculation = $"{request.Dividend} / {request.Divisor} = {quotient} (Rem: {remainder})";

            if (Request.Headers.TryGetValue("X-Evi-Tracking-Id", out var trackingId))
            {
                _journalService.AddEntry(trackingId, "Div", calculation);
            }

            return Ok(new DivResponse { Quotient = quotient, Remainder = remainder });
        }

        // POST /calculator/sqrt
        [HttpPost("sqrt")]
        public IActionResult Sqrt([FromBody] SqrtRequest request)
        {
            double result = Math.Sqrt(request.Number);
            string calculation = $"sqrt({request.Number}) = {result}";

            if (Request.Headers.TryGetValue("X-Evi-Tracking-Id", out var trackingId))
            {
                _journalService.AddEntry(trackingId, "Sqrt", calculation);
            }

            return Ok(new SqrtResponse { Square = result });
        }
    }
}