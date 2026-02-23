using Microsoft.AspNetCore.Mvc;
using CalculatorService.Server.Models;
using CalculatorService.Server.Services;

namespace CalculatorService.Server.Controllers
{
    [ApiController]
    [Route("journal")]
    public class JournalController : ControllerBase
    {
        private readonly JournalService _journalService;

        public JournalController(JournalService journalService)
        {
            _journalService = journalService;
        }

        [HttpPost("query")]
        public IActionResult Query([FromBody] JournalQueryRequest request)
        {
            var entries = _journalService.GetEntries(request.Id);
            return Ok(new JournalResponse { Operations = entries });
        }
    }
}