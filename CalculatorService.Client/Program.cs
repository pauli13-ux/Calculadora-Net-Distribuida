using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;

namespace CalculatorService.Client
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            client.BaseAddress = new Uri("http://localhost:5016/");

            bool salir = false;
            while (!salir)
            {
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine()?.Trim();

                switch (opcion)
                {
                    case "1": await ProcesarOperacionBinaria("+"); break;
                    case "2": await ProcesarOperacionBinaria("-"); break;
                    case "3": await ProcesarOperacionBinaria("*"); break;
                    case "4": await ProcesarOperacionBinaria("/"); break;
                    case "5": await ProcesarRaizCuadrada(); break;
                    case "6": await ConsultarHistorial(); break;
                    case "0": salir = true; break;
                    default:
                        Console.WriteLine("\n❌ Opción no válida. Intenta de nuevo.");
                        await EsperarTecla();
                        break;
                }
            }
        }

        static void MostrarMenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("*・゜・*:.。.*.。.:*・☆・゜・*:.。.*.。.:*・☆・");
            Console.WriteLine("                🧮 CALCULADORA  ");
            Console.WriteLine("*・゜・*:.。.*.。.:*・☆・゜・*:.。.*.。.:*・☆・");
            Console.WriteLine(" 1. ➕ Sumar");
            Console.WriteLine(" 2. ➖ Restar");
            Console.WriteLine(" 3. ✖️ Multiplicar");
            Console.WriteLine(" 4. ➗ Dividir");
            Console.WriteLine(" 5. 📐 Raíz Cuadrada");
            Console.WriteLine(" 6. 📜 Ver Historial (Journal)");
            Console.WriteLine(" 0. 🚪 Salir");
            Console.WriteLine("｡+ﾟ☆ﾟ+｡★｡+ﾟ☆ﾟ+｡★｡+ﾟ☆ﾟ+｡★｡+ﾟ☆ﾟ+｡");
            Console.Write("\nSelecciona una opción (0-6): ");
        }

        static async Task ProcesarOperacionBinaria(string simbolo)
        {
            try
            {
                string nombreOp = simbolo == "+" ? "SUMA" : simbolo == "-" ? "RESTA" : simbolo == "*" ? "MULTIPLICACIÓN" : "DIVISIÓN";
                Console.WriteLine($"\n--- Modulo de {nombreOp} ---");

                Console.Write("ID de seguimiento (ej: pau): ");
                string id = ConfigurarCabecera();

                double n1 = LeerNumero("Primer número: ");
                double n2 = LeerNumero("Segundo número: ");

                // Validación división por cero
                if (simbolo == "/" && n2 == 0)
                {
                    Console.WriteLine("❌ Error: No se puede dividir por cero.");
                }
                else
                {
                    HttpResponseMessage res = simbolo switch
                    {
                        "+" => await client.PostAsJsonAsync("calculator/add", new { Addends = new[] { n1, n2 } }),
                        "-" => await client.PostAsJsonAsync("calculator/sub", new { Minuend = n1, Subtrahend = n2 }),
                        "*" => await client.PostAsJsonAsync("calculator/mult", new { Factors = new[] { n1, n2 } }),
                        "/" => await client.PostAsJsonAsync("calculator/div", new { Dividend = n1, Divisor = n2 }),
                        _ => null
                    };

                    await MostrarResultado(res, simbolo);
                }
            }
            catch (Exception ex) { Console.WriteLine($"\n⚠️ Error: {ex.Message}"); }
            await EsperarTecla();
        }

        static async Task ProcesarRaizCuadrada()
        {
            try
            {
                Console.WriteLine("\n--- Modulo de RAÍZ CUADRADA ---");
                ConfigurarCabecera();
                double n1 = LeerNumero("Introduce el número: ");

                if (n1 < 0)
                {
                    Console.WriteLine("❌ Error: No existen raíces reales de números negativos.");
                }
                else
                {
                    var res = await client.PostAsJsonAsync("calculator/sqrt", new { Number = n1 });
                    var data = await res.Content.ReadFromJsonAsync<JsonElement>();
                    Console.WriteLine($"\n✅ Resultado: √{n1} = {data.GetProperty("square")}");
                }
            }
            catch (Exception ex) { Console.WriteLine($"\n⚠️ Error: {ex.Message}"); }
            await EsperarTecla();
        }

        static async Task ConsultarHistorial()
        {
            try
            {
                Console.Write("\nIntroduce el ID a consultar: ");
                string id = Console.ReadLine()?.Trim();

                var res = await client.PostAsJsonAsync("journal/query", new { Id = id });
                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadFromJsonAsync<JsonElement>();
                    Console.WriteLine($"\n--- 📜 HISTORIAL DE: {id.ToUpper()} ---");

                    if (data.TryGetProperty("operations", out JsonElement ops) && ops.GetArrayLength() > 0)
                    {
                        foreach (var item in ops.EnumerateArray())
                        {
                            Console.WriteLine($"• [{item.GetProperty("date")}] {item.GetProperty("operation")}: {item.GetProperty("calculation")}");
                        }
                    }
                    else { Console.WriteLine("No hay operaciones registradas."); }
                }
                else { Console.WriteLine("❌ Error al obtener el historial."); }
            }
            catch (Exception ex) { Console.WriteLine($"\n⚠️ Error: {ex.Message}"); }
            await EsperarTecla();
        }

        // --- MÉTODOS DE APOYO ---

        static string ConfigurarCabecera()
        {
            string id = Console.ReadLine()?.Trim();
            client.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(id)) client.DefaultRequestHeaders.Add("X-Evi-Tracking-Id", id);
            return id;
        }

        static double LeerNumero(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                if (double.TryParse(Console.ReadLine()?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double n)) return n;
                Console.WriteLine("❌ Formato incorrecto.");
            }
        }

        static async Task MostrarResultado(HttpResponseMessage res, string op)
        {
            if (res != null && res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadFromJsonAsync<JsonElement>();
                if (op == "/")
                {
                    Console.WriteLine($"\n✅ Cociente: {data.GetProperty("quotient")} | Resto: {data.GetProperty("remainder")}");
                }
                else
                {
                    string prop = op == "+" ? "sum" : op == "-" ? "difference" : "product";
                    Console.WriteLine($"\n✅ Resultado: {data.GetProperty(prop)}");
                }
            }
        }

        static async Task EsperarTecla()
        {
            Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
            await Task.Run(() => Console.ReadKey());
        }
    }
}