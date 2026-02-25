using Microsoft.AspNetCore.Mvc;
using CalculatorServiceok.Server.Services;
using CalculatorServiceok.Server.Models;
using System.Linq;

namespace CalculatorServiceok.Server.Controllers
{
    //1.

    [ApiController] //Tells .NET that this class is not a regular web page, but an API that communicates in JSON format.
    [Route("calculator")] //Defines the base URL. When communicating with this code, the address will always begin with http://your-server/calculator.
    public class CalculatorController : ControllerBase
    {

        //2.

        /*This is an external service responsible for storing the history. The controller doesn't know "how" it's stored (whether in a database or in memory); it only knows that it has to call _journalService.AddEntry.
         By including it in the constructor, the system automatically passes it to the controller when it's created.*/
        
        private readonly JournalService _journalService;
        // Definimos la constante para no repetir texto y evitar errores de dedo
        private const string TRACKING = "X-Evi-Tracking-Id";

        public CalculatorController(JournalService journalService)
        {
            _journalService = journalService;
        }

        //3.
        /*[HttpPost("add")]: Indicates that this method only responds to "POST" type messages in the /calculator/add path.
            [FromBody] AddRequest: The server opens the JSON package contained in the message body and extracts the numbers.
            [FromHeader(Name = TRACKING)]: This is where the server looks for your "signature" or ID (pau, juan, etc.) in the message envelope.*/

        [HttpPost("add")]
        public IActionResult Add([FromBody] AddRequest request, [FromHeader(Name = TRACKING)] string? id)
        {
            // Addition logic using LINQ
            /*Calculation: `var res = request.Addends.Sum();` (Sums all received numbers).
            Logging: If the user submitted an ID, a descriptive phrase is created (e.g., "5 + 5 = 10") and saved to the journaling service.
            Response: `return Ok(new { result = res });` (Sends a 200 success code and the result in JSON format).*/
           
            //FIXME: calculator service
            var res = request.Addends.Sum();

            // Si hay ID, guardamos en el diario
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
            var res = request.Minuend - request.Subtrahend;

            if (!string.IsNullOrWhiteSpace(id))
                _journalService.AddEntry(id, "Resta", $"{request.Minuend} - {request.Subtrahend} = {res}");

            return Ok(new { result = res });
        }

        [HttpPost("mult")]
        public IActionResult Multiply([FromBody] MultRequest request, [FromHeader(Name = TRACKING)] string? id)
        {
            // Aggregate multiplies all the numbers in the list one by one
            var res = request.Factors.Aggregate(1.0, (a, b) => a * b);

            if (!string.IsNullOrWhiteSpace(id))
                _journalService.AddEntry(id, "Mult", $"{string.Join(" * ", request.Factors)} = {res}");

            return Ok(new { result = res });
        }

        [HttpPost("div")]
        public IActionResult Divide([FromBody] DivRequest request, [FromHeader(Name = TRACKING)] string? id)
        {
            // Important validation: We cannot divide by zero!
            if (request.Divisor == 0) return BadRequest(new { error = "División por cero no permitida" });

            var quotient = request.Dividend / request.Divisor;
            var remainder = request.Dividend % request.Divisor;

            if (!string.IsNullOrWhiteSpace(id))
                _journalService.AddEntry(id, "Div", $"{request.Dividend} / {request.Divisor} = Q:{quotient} R:{remainder}");

            return Ok(new { quotient, remainder });
        }

        [HttpPost("sqrt")]
        public IActionResult SquareRoot([FromBody] SqrtRequest request, [FromHeader(Name = TRACKING)] string? id)
        {
            // The square root of negative numbers is not real
            if (request.Number < 0) return BadRequest(new { error = "No se puede calcular la raíz de un número negativo" }); //If you attempt something mathematically impossible, the server doesn't break; it simply returns a BadRequest (Error 400), warning you that your request is malformed.

            var res = Math.Sqrt(request.Number);

            if (!string.IsNullOrWhiteSpace(id))
                _journalService.AddEntry(id, "Raiz", $"sqrt({request.Number}) = {res}");

            return Ok(new { result = res });
        }

        //Unlike the others, this is a GET method because we are only "requesting information," not sending data for processing. It searches the JournalService for all entries that match the ID and returns them in a list.
        
                [HttpGet("journal/{id}")]
        public IActionResult GetJournal(string id)
        {
            var entries = _journalService.GetEntries(id);
            // If the user has no operations, we return an empty list but with an OK.
            return Ok(new { operations = entries });
        }
    }
}


/*☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～
NOTAS PARA MI:) 

Controller (el cerebro del servidor)
Su función es ser la "puerta de entrada" del servidor. 
Recibe las peticiones de los clientes (como la calculadora), decide qué operación hacer y guarda el rastro en un historial.
 
Resumen del flujo de datos:

Llega la petición a /calculator/mult.
El Controlador lee los factores del cuerpo y el ID de la cabecera.
El Controlador multiplica los números usando Aggregate.
El Controlador le dice al JournalService: "Apunta que el usuario X multiplicó esto".
El Controlador te devuelve el resultado final.
 
*/