using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalculatorService.Client
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:62030/")
        };

        private const string DIVIDIR = "/";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            bool salir = false;
            while (!salir)
            {
                MostrarMenuPrincipal(); // Recuperamos tu menú original
                var opcion = Console.ReadLine()?.Trim();

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
                        Console.WriteLine("\n❌ Opción no válida.");
                        // FIXME: why? say me something to know why is invalid my request
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
            Console.WriteLine("Selecciona una opción (0-6): ");
        }

        static async Task ProcesarOperacionBinaria(string simbolo)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("--- Modulo de Operación ---");
                Console.Write("ID de seguimiento (ej: pau): ");
                string id = ConfigurarCabecera();

                double n1 = LeerNumero("Primer número: ");
                double n2 = LeerNumero("Segundo número: ");

                HttpResponseMessage res = simbolo switch
                {
                    "+" => await client.PostAsJsonAsync("calculator/add", new AddRequest() { Addends = new[] { n1, n2 } }),
                    "-" => await client.PostAsJsonAsync("calculator/sub", new { Minuend = n1, Subtrahend = n2 }),
                    "*" => await client.PostAsJsonAsync("calculator/mult", new { Factors = new[] { n1, n2 } }),
                    "/" => await client.PostAsJsonAsync("calculator/div", new { Dividend = n1, Divisor = n2 }),
                    _ => null
                };

                await MostrarResultado(res, simbolo);
            }
            catch (Exception ex) { Console.WriteLine($"\n⚠️ Error: {ex.Message}"); }
            await EsperarTecla();
        }

        // --- LOS MÉTODOS DE APOYO QUE TE GUSTABAN ---
        private static string ConfigurarCabecera()
        {
            string id = Console.ReadLine()?.Trim();
            client.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrWhiteSpace(id) && string.IsNullOrEmpty(id)) client.DefaultRequestHeaders.Add("X-Evi-Tracking-Id", id);
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

            switch (op)
            {
                case DIVIDIR:
                    var result = await res.Content.ReadFromJsonAsync<DivResponse>();
                    Console.WriteLine($"\n✅ Cociente: {resutl.Quotient} | Resto: {data.GetProperty("remainder")}");
                default:
                    break;
            }

            if (res != null && res.IsSuccessStatusCode)
            {
                var data = await res.Content.ReadFromJsonAsync<JsonElement>();
                if (op == DIVIDIR)
                    Console.WriteLine($"\n✅ Cociente: {data.GetProperty("quotient")} | Resto: {data.GetProperty("remainder")}");
                else
                    // Aquí usamos "result" porque es lo que envía el servidor corregido
                    Console.WriteLine($"\n✅ Resultado: {data.GetProperty("result")}");
            }
            else { Console.WriteLine("❌ Error al procesar."); }
        }

        static async Task ProcesarRaizCuadrada() { /* código similar a binaria */ }

        static async Task ConsultarHistorial()
        {
            Console.Write("\nID a consultar: ");
            string id = Console.ReadLine();
            var res = await client.GetAsync($"calculator/journal/{id}");
            if (res.IsSuccessStatusCode)
            {
                var lista = await res.Content.ReadFromJsonAsync<List<JsonElement>>();
                Console.WriteLine($"\n--- 📜 HISTORIAL DE: {id.ToUpper()} ---");
                foreach (var item in lista)
                    Console.WriteLine($"• {item.GetProperty("operation")}: {item.GetProperty("calculation")}");
            }
            else { Console.WriteLine("❌ No hay historial."); }
            await EsperarTecla();
        }

        static async Task EsperarTecla()
        {
            Console.WriteLine("\nPresiona una tecla para continuar...");
            await Task.Run(() => Console.ReadKey());
        }
    }
}