using CalculatorServiceok.Server.Models;
using System.Collections.Concurrent;
using System.Security;

namespace CalculatorServiceok.Server.Services
{

    // This is like the "contract" that allows the controller to use this service.

    /*Think of a wall socket. The socket is the interface.
      The socket defines the shape (two or three prongs) and the voltage.
      It doesn't matter to you whether the electricity behind the wall comes from a solar panel, a nuclear power plant, or a gasoline generator.
      As long as the device (your phone charger) is compatible with the interface (the shape of the socket), it will work.*/

    public interface IJournalService
    {
        void AddEntry(string id, string operation, string calculation);
        List<JournalEntry> GetEntries(string id);
        void ClearJournal(string id); // Un extra útil
    }

    public class JournalService : IJournalService
    {
        // The data store: Key = User name, Value = List of their operations
        private static readonly ConcurrentDictionary<string, List<JournalEntry>> _journals = new(); //It's like a giant filing cabinet. The folder labels are the IDs (usernames), and inside each folder is a List (sheets of paper with the entries).

        public void AddEntry(string id, string operation, string calculation)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            // We search for the user's list. If it doesn't exist, we create a new one.
            var list = _journals.GetOrAdd(id, _ => new List<JournalEntry>()); //This is a very efficient line. It says: "Find Pau's folder. It's not there? Then create one right now and give it to me." All in one step.


            // We use lock so that if two additions finish at the same time,
            // they don't try to write to the same list at the same millisecond.

            lock (list)
            {
                list.Add(new JournalEntry
                {
                    Operation = operation,
                    Calculation = calculation,
                    Date = DateTime.Now
                });
            }
        }

        public List<JournalEntry> GetEntries(string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && _journals.TryGetValue(id, out var entries))
            {
                // We return a copy (.ToList) so that no one can modify
                // the original list from outside without permission.
                lock (entries)
                {
                    return entries.OrderByDescending(e => e.Date).ToList();
                }
            }
            return new List<JournalEntry>();
        }

        // Extra method: In case the user wants to erase their trace.
        public void ClearJournal(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                _journals.TryRemove(id, out _);
            }
        }
    }
}

/*☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～☆～
NOTAS PARA MI:) 

Thread-Safety (Seguridad de hilos): En un servidor, 100 personas pueden pulsar "Sumar" a la vez. 
Usar ConcurrentDictionary y lock asegura que el servidor no se vuelva loco intentando escribir en el mismo sitio de la memoria simultáneamente.

Encapsulamiento: Al usar .ToList() en GetEntries, estás enviando una foto del historial en ese momento. 
Si el usuario sigue haciendo cálculos después, esa foto no cambia. Esto evita errores de sincronización.

.OrderByDescending(e => e.Date). se verá lo más reciente primero.
 
ConcurrentDictionary<string, List<JournalEntry>>: Es como un archivador gigante. 
Las etiquetas de las carpetas son los id (nombres de usuario) y dentro de cada carpeta hay una List (hojas de papel con las operaciones).

GetOrAdd: Es una línea muy eficiente. Dice: "Busca la carpeta de Pau. 
¿No está? Pues crea una ahora mismo y dámela". Todo en un solo paso.

lock (list): Imagina que el historial es un cuaderno. El lock es como decir: 
"Solo una persona puede tener el bolígrafo a la vez". Así evitamos que dos mensajes se escriban uno encima del otro.

TryGetValue: Es la forma segura de leer. Si pides el historial de alguien que no existe, 
simplemente te dice "No lo tengo" (false) en lugar de lanzar un error que cierre el programa.



En tu código, el Controlador (CalculatorController) no pide directamente la clase JournalService, 
pide la interfaz IJournalService. Esto tiene tres ventajas enormes:

Flexibilidad: Hoy guardas los datos en la memoria RAM (ConcurrentDictionary). 
Pero si mañana decides guardarlos en una Base de Datos SQL, solo tienes que crear una nueva clase que cumpla el contrato IJournalService. No tendrás que tocar ni una sola línea de código de tus controladores.

Desacoplamiento: El Controlador y el Servicio no están "pegados". 
Son piezas de LEGO que encajan porque comparten la misma conexión.

Inyección de Dependencias: Es lo que permite que en el archivo Program.cs tú digas: 
"Cuando alguien pida el contrato IJournalService, entrégale la clase JournalService".


-La Interfaz (IJournalService): El contrato (las reglas).
-La Clase (JournalService): El trabajador que cumple el contrato (el código real).
-El Cliente (Controller): El que usa al trabajador siguiendo las reglas del contrato.

En resumen:
La interfaz es una forma de organizar el caos. Asegura que, sin importar cuántas personas trabajen en el 
código o cuánto cambie el sistema en el futuro, las piezas siempre encajarán porque todas respetan el mismo "contrato".
 
 
 */