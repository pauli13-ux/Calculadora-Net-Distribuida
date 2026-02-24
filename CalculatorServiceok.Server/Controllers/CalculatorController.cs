using Microsoft.AspNetCore.Mvc;
using CalculatorServiceok.Server.Services;
using CalculatorServiceok.Server.Models;

namespace CalculatorServiceok.Server.Controllers
{
    [ApiController]
    [Route("calculator")]
    public class CalculatorController : ControllerBase
    {
        private readonly JournalService _journalService;
        private const string TRACKING = "X-Evi-Tracking-Id";

        public CalculatorController(JournalService journalService)
        {
            _journalService = journalService;
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] AddRequest request, [FromHeader(Name = TRACKING)] string id)
        {
            var res = _calculatorService.Sum(request);
            //var res = request.Addends.Sum();
            if (!string.IsNullOrWhiteSpace(id))
                _journalService.AddEntry(id, "Suma", request, res); // $"{string.Join(" + ", request.Addends)} = {res}");
            return Ok(new { result = res });
        }

        [HttpPost("sub")]
        public IActionResult Subtract([FromBody] SubRequest request, [FromHeader(Name = "X-Evi-Tracking-Id")] string? id)
        {
            var res = request.Minuend - request.Subtrahend;
            _journalService.AddEntry(id ?? "anonimo", "Resta", $"{request.Minuend} - {request.Subtrahend} = {res}");
            return Ok(new { result = res });
        }

        [HttpPost("mult")]
        public IActionResult Multiply([FromBody] MultRequest request, [FromHeader(Name = "X-Evi-Tracking-Id")] string? id)
        {
            var res = request.Factors.Aggregate(1.0, (a, b) => a * b);
            _journalService.AddEntry(id ?? "anonimo", "Mult", $"{string.Join(" * ", request.Factors)} = {res}");
            return Ok(new { result = res });
        }

        [HttpPost("div")]
        public IActionResult Divide([FromBody] DivRequest request, [FromHeader(Name = "X-Evi-Tracking-Id")] string? id)
        {
            if (request.Divisor == 0) return BadRequest("División por cero");
            var quotient = request.Dividend / request.Divisor;
            var remainder = request.Dividend % request.Divisor;
            _journalService.AddEntry(id ?? "anonimo", "Div", $"{request.Dividend} / {request.Divisor} = Q:{quotient} R:{remainder}");
            return Ok(new { quotient, remainder });
        }

        [HttpPost("sqrt")]
        public IActionResult SquareRoot([FromBody] SqrtRequest request, [FromHeader(Name = "X-Evi-Tracking-Id")] string? id)
        {
            var res = Math.Sqrt(request.Number);
            _journalService.AddEntry(id ?? "anonimo", "Raiz", $"sqrt({request.Number}) = {res}");
            return Ok(new { result = res });
        }

        [HttpGet("journal/{id}")]
        public IActionResult GetJournal(string id)
        {
            return Ok(_journalService.GetEntries(id));
        }
    }
}