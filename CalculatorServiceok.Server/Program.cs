#nullable disable

using CalculatorServiceok.Server.Services;
using CalculatorServiceok.Server.Models;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// 2. Configure Serilog before builder.Build()
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // 📅 Daily rotation here
	.CreateLogger();

builder.Host.UseSerilog(); // 3. Dile al servidor que use Serilog

// --- 1. SERVICE CONFIGURATION (Dependency Injection) ---

// CORRECTION: We registered the interface linked to the class.
// AddSingleton ensures that all users share the same history in memory.

builder.Services.AddSingleton<IJournalService, JournalService>(); ;

builder.Services.AddSingleton<ICalculatorService, CalculatorService>();

builder.Services.AddControllers();

// OPTIONAL: Add Swagger (it helps a lot for testing the API from the browser)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 2. MIDDLEWARE CONFIGURATION (Order matters) ---

// If we are in development, we activate the Swagger visual interfaceif (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Important for the API to respond correctly
app.UseAuthorization();

// Map the controllers
app.MapControllers();

app.Run();


/*☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～
NOTAS PARA MI:) 

Como he hecho el cambio de añadir IJournalService tengo que cambiar la forma en la que registro el servicio.

1. Registro por Interfaz (AddSingleton<IJournalService, JournalService>)
En tu código pusiste AddSingleton<JournalService>. Aunque funciona, lo correcto es registrarlo con su interfaz.

Por qué: Si mañana creas un SqlJournalService, solo cambias esa línea a AddSingleton<IJournalService, SqlJournalService>() 
y el resto de tu aplicación seguirá funcionando sin tocar nada. Eso es el poder de la inyección de dependencias.

2. Swagger (AddSwaggerGen y UseSwaggerUI)
He añadido Swagger. Es una herramienta casi obligatoria. Cuando ejecutes el servidor, 
si vas a la URL http://localhost:XXXX/swagger, verás una página web interactiva donde puedes probar todos tus métodos 
(Suma, Resta, Journal) sin necesidad de usar el cliente de consola.

3. Middlewares de Seguridad y Entorno
He añadido app.UseAuthorization() y la validación de IsDevelopment(). Esto prepara tu código para que,
si el día de mañana quieres ponerlo en internet, tenga la estructura necesaria para ser seguro y eficiente.

Explicación del "Ciclo de Vida" de este código:

builder: Es como el arquitecto. Aquí le dices qué herramientas vas a usar (Controllers, Servicios).
AddSingleton: Le dice al arquitecto: "Crea una única caja de archivos (JournalService) y dásela a todo el que la pida".
app: Es el edificio ya construido.
MapControllers: Es el recepcionista que dice: "Si alguien viene preguntando por /calculator, llévalo al CalculatorController".
 
 */