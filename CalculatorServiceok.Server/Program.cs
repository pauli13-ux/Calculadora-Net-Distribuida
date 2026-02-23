using CalculatorService.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Esta línea es la que evita el error "Unable to resolve service"
builder.Services.AddSingleton<JournalService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();