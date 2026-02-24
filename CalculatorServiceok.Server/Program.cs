using CalculatorServiceok.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar el servicio
builder.Services.AddSingleton<JournalService>();
builder.Services.AddControllers();

var app = builder.Build();

// 2. Mapear los controladores
app.MapControllers();

app.Run();