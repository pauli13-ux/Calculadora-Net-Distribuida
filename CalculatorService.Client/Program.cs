
#nullable disable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalculatorService.Client
{
	class Program
	{
		// Mantenemos tu cliente con la dirección base
		private static readonly HttpClient client = new HttpClient()
		{
			BaseAddress = new Uri("http://localhost:62030/") //This is the address of the "office" to which we send the requests.
		};

		private const string DIVIDIR = "/";

		static async Task Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;

			bool salir = false;
			while (!salir) /*This creates an infinite loop that only breaks if you choose option "0".
							Inside the switch statement, depending on the number you press, the program jumps to a specific function:
							If you press 1, 2, 3, or 4, you will run `ProcessBinaryOperation`.
							If you press 5, you will run `ProcessSquareRoot`.
							If you press 6, you will view your history.*/

			{
				MostrarMenuPrincipal();
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
						// FIXME: explain why it is invalid
						Console.WriteLine($"\n❌ '{opcion}' no es una opción válida del menú (0-6).");
						await EsperarTecla();
						break;
				}
			}
		}

		static void MostrarMenuPrincipal()
		{
			Console.Clear();
			Console.WriteLine("*・゜・*:.。.*.。.:*・☆・゜・*:.。.*.。.:*・☆・");
			Console.WriteLine("                🧮 CALCULADORA REST          ");
			Console.WriteLine("*・゜・*:.。.*.。.:*・☆・゜・*:.。.*.。.:*・☆・");
			Console.WriteLine(" 1. ➕ Sumar");
			Console.WriteLine(" 2. ➖ Restar");
			Console.WriteLine(" 3. ✖️ Multiplicar");
			Console.WriteLine(" 4. ➗ Dividir");
			Console.WriteLine(" 5. 📐 Raíz Cuadrada");
			Console.WriteLine(" 6. 📜 Ver Historial (Journal)");
			Console.WriteLine(" 0. 🚪 Salir");
			Console.WriteLine("｡+ﾟ☆ﾟ+｡★｡+ﾟ☆ﾟ+｡★｡+ﾟ☆ﾟ+｡★｡+ﾟ☆ﾟ+｡");
			Console.Write("Selecciona una opción: ");
		}

		static async Task ProcesarOperacionBinaria(string simbolo)
		{
			try
			{
				Console.WriteLine($"\n--- Módulo de {simbolo} ---");
				ConfigurarCabecera(); // pide ID bien, clean any previous IDs.If you type anything, it adds it to the client configuration. This way, the server knows that the next sum should be recorded in the "pau" history.

				/*It's a "bodyguard" function. It uses a while(true) loop so that if the user types letters instead of numbers, the program doesn't crash; it simply tells them "Incorrect format" and asks for the number again until it's valid.*/

				double n1 = LeerNumero("Primer número: ");
				double n2 = LeerNumero("Segundo número: ");

				HttpResponseMessage res = simbolo switch

				//Sending: When you add, the code creates a "package" like this: {"Addends": [5, 10]} and sends it using PostAsJsonAsync.

				{
					"+" => await client.PostAsJsonAsync("calculator/add", new { Addends = new[] { n1, n2 } }),
					"-" => await client.PostAsJsonAsync("calculator/sub", new { Minuend = n1, Subtrahend = n2 }),
					"*" => await client.PostAsJsonAsync("calculator/mult", new { Factors = new[] { n1, n2 } }),
					"/" => await client.PostAsJsonAsync("calculator/div", new { Dividend = n1, Divisor = n2 }),
					_ => null
				};

				/*It's a smart function that knows what to display depending on the operation:
                If it's division, it looks for two values ​​in the answer: the quotient and the remainder.
                If it's another operation, it looks for a single value called "result".*/

				await MostrarResultado(res, simbolo);
			}
			// FIXME: Error handling??
			catch (Exception ex) { Console.WriteLine($"\n⚠️ Error: {ex.Message}"); }
			await EsperarTecla();
		}


		private static void ConfigurarCabecera()
		{
			Console.Write("ID de seguimiento (ej: pau): ");
			string id = Console.ReadLine()?.Trim();

			client.DefaultRequestHeaders.Remove("X-Evi-Tracking-Id"); //This is a custom header. It's like putting your name on an envelope so the server knows that these calculations are yours and saves them in your personal folder (the "Journal").
																	  // If it's not null or a space, we add it.
			if (!string.IsNullOrWhiteSpace(id))
			{
				client.DefaultRequestHeaders.Add("X-Evi-Tracking-Id", id);
			}
		}

		static double LeerNumero(string mensaje)
		{
			while (true)
			{
				Console.Write(mensaje);
				string entrada = Console.ReadLine()?.Replace(",", ".");
				if (double.TryParse(entrada, NumberStyles.Any, CultureInfo.InvariantCulture, out double n))
					return n;
				Console.WriteLine("❌ Formato incorrecto. Usa números (ej: 5.5).");
			}
		}

		//Variable correction and division logic
		static async Task MostrarResultado(HttpResponseMessage res, string op)
		{
			if (res != null && res.IsSuccessStatusCode)
			{
				var data = await res.Content.ReadFromJsonAsync<JsonElement>(); //The server responds and the code uses ReadFromJsonAsync<JsonElement> to read the response.

				if (op == DIVIDIR)
				{
					// Corrected: "result" -> "quotient" and "remainder"
					var q = data.GetProperty("quotient");
					var r = data.GetProperty("remainder");
					Console.WriteLine($"\n✅ Cociente: {q} | Resto: {r}");
				}
				else if (op == "sqrt")
				{
					Console.WriteLine($"\n✅ Raíz Cuadrada: {data.GetProperty("result")}");
				}
				else
				{
					Console.WriteLine($"\n✅ Resultado: {data.GetProperty("result")}");
				}
			}
			else
			{
				Console.WriteLine($"\n❌ Error al procesar: {res?.StatusCode}");
			}
		}

		// Square Root Method 
		static async Task ProcesarRaizCuadrada()
		{
			try
			{
				Console.WriteLine("\n--- Módulo de Raíz Cuadrada ---");
				ConfigurarCabecera();
				double n = LeerNumero("Introduce el número: ");

				var res = await client.PostAsJsonAsync("calculator/sqrt", new { Number = n });
				await MostrarResultado(res, "sqrt");
			}
			catch (Exception ex) { Console.WriteLine($"\n⚠️ Error: {ex.Message}"); }
			await EsperarTecla();
		}

		// journal logic
		static async Task ConsultarHistorial()
		{
			Console.Write("\nIntroduce el ID a consultar (ej: pau): ");
			string id = Console.ReadLine()?.Trim();

			if (string.IsNullOrEmpty(id))
			{
				Console.WriteLine("❌ Debes introducir un ID válido.");
			}
			else
			{
				var res = await client.GetAsync($"calculator/journal/{id}");
				if (res.IsSuccessStatusCode)
				{
					// The server usually returns an object with a list called "operations"
					var doc = await res.Content.ReadFromJsonAsync<JsonElement>();
					Console.WriteLine($"\n--- 📜 HISTORIAL DE: {id.ToUpper()} ---");

					if (doc.TryGetProperty("operations", out JsonElement operations))
					{
						foreach (var item in operations.EnumerateArray())
						{
							Console.WriteLine($"• {item.GetProperty("operation")}: {item.GetProperty("calculation")}");
						}
					}
				}
				else { Console.WriteLine($"\n❌ No se encontró historial para el ID: {id}"); }
			}
			await EsperarTecla();
		}

		static async Task EsperarTecla()
		{
			Console.WriteLine("\nPresiona una tecla para continuar...");
			await Task.Run(() => Console.ReadKey(true));
		}
	}
}



/*☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～
 NOTAS PARA MI:) 

Flujo de una operación
Para que lo veas claro, este es el camino que sigue el programa cuando quieres sumar:
Tú: Eliges "1" en el menú.
Código: Te pide el ID de seguimiento.
Tú: Escribes "juan".
Código: Te pide los números.
Tú: Escribes 10 y 20.
Código: Empaqueta todo en un JSON, le pone la etiqueta "juan" y lo envía a la URL /calculator/add.
Servidor: Suma, guarda en la base de datos de "juan" y devuelve {"result": 30}.
Código: Recibe ese 30 y te lo muestra con un ✅ verde.

 
Al ser una Arquitectura Cliente-Servidor:
-Varios usuarios pueden usar la misma calculadora a la vez.
-Los cálculos no gastan batería de tu dispositivo (los hace el servidor).
-Tu historial se guarda en la "nube" (el servidor) y puedes consultarlo desde cualquier otro cliente con tu ID.

 4 pilares fundamentales que lo mantienen en pie. Asi funciona cualquier aplicación que se conecta a internet.

1. El uso de Cabeceras Personalizadas (Custom Headers)
No solo envías datos, envías metadatos.
client.DefaultRequestHeaders.Add("X-Evi-Tracking-Id", id);
Para autenticación (tokens) o para identificar al usuario. Sin esto, el servidor no sabría que la operación de "Suma" pertenece al historial de "Pau" y no al de "Juan".

2. La Serialización y Deserialización JSON
El código actúa como un traductor constante.
Hacia el servidor: Usas PostAsJsonAsync. C# convierte tu objeto (como new { Addends = ... }) en un texto que el servidor entiende: {"addends":[1,2]}.
Desde el servidor: Usas ReadFromJsonAsync<JsonElement>. C# recibe un texto plano y lo convierte en un objeto que puedes manipular para sacar el resultado.

3. El Manejo de la Asincronía (Task, async, await)
Evita que la aplicación se "congele".
Importancia: En una aplicación real, si el servidor tarda 5 segundos en responder y no usas await, el usuario pensaría que el programa se ha roto porque no podría ni mover la ventana ni escribir. Con await, el hilo principal queda libre para que la interfaz siga viva.

4. Robustez en la Entrada de Datos (TryParse)
double.TryParse(..., out double n)
Si usaras un simple Convert.ToDouble(), el programa explotaría (crash) en cuanto el usuario escribiera una letra por error. El TryParse permite que el programa gestione el error amigablemente y pida el dato de nuevo sin cerrarse.

Resumen de la arquitectura:
Modelo Request-Response (Petición-Respuesta):

Preparación: Configuras quién eres (Header).
Petición: Envías qué quieres hacer (POST/GET con JSON).
Espera: El programa aguanta sin bloquearse (await).
Procesado: Recibes la respuesta y extraes el valor con GetProperty.*/