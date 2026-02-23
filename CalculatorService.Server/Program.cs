using CalculatorService.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<JournalService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();