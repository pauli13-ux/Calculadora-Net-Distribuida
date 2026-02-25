using Microsoft.AspNetCore.Mvc;
using CalculatorServiceok.Server.Services;
using CalculatorServiceok.Server.Models;
using System.Linq;

namespace CalculatorServiceok.Server.Controllers
{
	[ApiController]
	[Route("calculator")]
	public class CalculatorController : ControllerBase
	{
		private readonly ILogger<CalculatorController> _logger;
		private readonly IJournalService _journalService;
		private readonly ICalculatorService _calculatorService;
		private const string TRACKING = "X-Evi-Tracking-Id";

		public CalculatorController(
			ILogger<CalculatorController> logger,
			IJournalService journalService,
			ICalculatorService calculatorService)
		{
			_logger = logger;
			_journalService = journalService;
			_calculatorService = calculatorService;
		}

		[HttpPost("add")]
		public IActionResult Add([FromBody] AddRequest request, [FromHeader(Name = TRACKING)] string? id)
		{
			var res = _calculatorService.Add(request.Addends);

			_logger.LogInformation("Suma realizada. ID: {Id}. Resultado: {Res}", id ?? "N/A", res);

			if (!string.IsNullOrWhiteSpace(id))
			{
				string calculation = $"{string.Join(" + ", request.Addends)} = {res}";
				_journalService.AddEntry(id, "Suma", calculation);
			}

			return Ok(new { result = res });
		}

		[HttpPost("sub")]
		public IActionResult Subtract([FromBody] SubRequest request, [FromHeader(Name = TRACKING)] string? id)
		{
			var res = _calculatorService.Subtract(request.Minuend, request.Subtrahend);

			_logger.LogInformation("Resta realizada. ID: {Id}. Resultado: {Res}", id ?? "N/A", res);

			if (!string.IsNullOrWhiteSpace(id))
				_journalService.AddEntry(id, "Resta", $"{request.Minuend} - {request.Subtrahend} = {res}");

			return Ok(new { result = res });
		}

		[HttpPost("mult")]
		public IActionResult Multiply([FromBody] MultRequest request, [FromHeader(Name = TRACKING)] string? id)
		{
			var res = _calculatorService.Multiply(request.Factors);

			_logger.LogInformation("Multiplicación realizada. ID: {Id}. Resultado: {Res}", id ?? "N/A", res);

			if (!string.IsNullOrWhiteSpace(id))
				_journalService.AddEntry(id, "Mult", $"{string.Join(" * ", request.Factors)} = {res}");

			return Ok(new { result = res });
		}

		[HttpPost("div")]
		public IActionResult Divide([FromBody] DivRequest request, [FromHeader(Name = TRACKING)] string? id)
		{
			if (request.Divisor == 0)
			{
				_logger.LogWarning("Intento de división por cero. ID: {Id}", id ?? "N/A");
				return BadRequest(new { error = "División por cero no permitida" });
			}

			var (quotient, remainder) = _calculatorService.Divide(request.Dividend, request.Divisor);

			_logger.LogInformation("División realizada. ID: {Id}. Q:{Q} R:{R}", id ?? "N/A", quotient, remainder);

			if (!string.IsNullOrWhiteSpace(id))
				_journalService.AddEntry(id, "Div", $"{request.Dividend} / {request.Divisor} = Q:{quotient} R:{remainder}");

			return Ok(new { quotient, remainder });
		}

		[HttpPost("sqrt")]
		public IActionResult SquareRoot([FromBody] SqrtRequest request, [FromHeader(Name = TRACKING)] string? id)
		{
			if (request.Number < 0)
			{
				_logger.LogWarning("Intento de raíz negativa. ID: {Id}", id ?? "N/A");
				return BadRequest(new { error = "No se puede calcular la raíz de un número negativo" });
			}

			var res = _calculatorService.SquareRoot(request.Number);

			_logger.LogInformation("Raíz realizada. ID: {Id}. Res: {Res}", id ?? "N/A", res);

			if (!string.IsNullOrWhiteSpace(id))
				_journalService.AddEntry(id, "Raiz", $"sqrt({request.Number}) = {res}");

			return Ok(new { result = res });
		}

		[HttpGet("journal/{id}")]
		public IActionResult GetJournal(string id)
		{
			_logger.LogInformation("Consulta historial ID: {Id}", id);
			var entries = _journalService.GetEntries(id);
			return Ok(new { operations = entries });
		}
	}
}