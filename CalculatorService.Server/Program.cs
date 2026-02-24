/*using CalculatorService.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Registro del servicio único para el historial
builder.Services.AddSingleton<JournalService>();

// Importante para que el servidor encuentre tus controladores
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();*/