using Microsoft.AspNetCore.Mvc;
using CalculatorServiceok.Server.Services;
using CalculatorServiceok.Server.Models;
using System.Collections.Generic;

namespace CalculatorServiceok.Server.Controllers
{
    [ApiController]
    [Route("journal")] //This handler responds at the address /journal. If you're looking for the history for "pau", the URL will be GET http://localhost:XXXX/journal/pau.
    public class JournalController : ControllerBase
    {
        private readonly JournalService _journalService;

        public JournalController(JournalService journalService)
        {
            _journalService = journalService;
        }

        // GET: /journal/{id}
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            // 1. Basic security validation
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { error = "El ID de seguimiento es obligatorio." });
            }

            // 2. We recovered the service tickets
            var entries = _journalService.GetEntries(id); //This line is the one that actually does the work. It goes to the service (which is usually an in-memory list or a database) and retrieves all the rows that have that ID.

            // 3. We check if there are records
            if (entries == null || !entries.Any())
            {
                /*We return 404 if the ID doesn't exist, or simply an empty list.
                The most common practice in APIs is to return an empty list with a 200 OK response.*/
                return Ok(new { operations = new List<object>() }); //This is the message that says: "Everything went well, here's the data." It sends an HTTP 200 status code.
            }

            // 4. Completado: Devolvemos un objeto con una propiedad "operations" 
            // para que coincida con lo que el cliente espera recibir.
            return Ok(new { operations = entries });
        }
    }
}


/*☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～
NOTAS PARA MI:) 

Controlador secundario dedicado exclusivamente a consultar el historial. 

Mientras que el controlador anterior (CalculatorController) se encarga de hacer los cálculos (sumar, restar, etc.), 
este se encarga exclusivamente de consultar la memoria del servidor.

1. Su Responsabilidad Única (Journaling)
Solo sirve para leer el historial.
No sabe sumar, no sabe restar, ni le importa. Solo sabe recibir un nombre (ID), 
preguntarle al servicio de diario qué ha hecho esa persona y entregárselo al usuario.

2. El punto de entrada (Route)
Define el "pasillo" del servidor. Si alguien toca a la puerta en la dirección /journal, .NET sabe que debe llamar a esta clase. 
Es una forma de organizar la API para que no todo esté mezclado en un solo lugar.

3. El buscador por ID (HttpGet)
La llave {id} entre llaves indica que es una variable.
Si el cliente pide /journal/pau, la variable id valdrá "pau".
Si pide /journal/pedro, valdrá "pedro".
Es como una ficha de biblioteca: el controlador toma el nombre que tú le des y lo usa para buscar en los archivos.  
 
4. La comunicación con el Servicio
var entries = _journalService.GetEntries(id);  
El controlador no guarda los datos él mismo. El controlador es como el recepcionista de un hotel: 
tú le pides tu llave, él se da la vuelta, la coge del casillero (que sería el JournalService) y te la entrega.

5. La respuesta al Cliente (Ok)
return Ok(new { operations = entries });

El controlador empaqueta la información. No te tira los datos a la cara; los pone en un formato JSON limpio y 
ordenado para que el programa cliente (la calculadora de consola) pueda leerlos fácilmente.

Si encuentra datos, envía un Código 200 (OK).
Si algo está mal, envía un Código 400 (Bad Request). 

Sin este controlador, podrías hacer mil cálculos, pero nunca podrías volver a verlos. 
Es la ventana que permite al usuario mirar dentro de la base de datos del servidor y decir: "A ver, ¿qué operaciones hice ayer con el ID 'pau'?".
  
*/