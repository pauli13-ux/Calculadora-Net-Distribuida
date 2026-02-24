using Microsoft.AspNetCore.Mvc;
using CalculatorServiceok.Server.Services;
using CalculatorServiceok.Server.Models;

namespace CalculatorServiceok.Server.Controllers
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

        [HttpGet("{id}")] // Esto crea la ruta /journal/tu-nombre
        public IActionResult Get(string id)
        {
            var entries = _journalService.GetEntries(id);
            return Ok(entries);
        }
    }
}