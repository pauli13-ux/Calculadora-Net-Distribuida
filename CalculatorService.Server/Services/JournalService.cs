using System.Collections.Concurrent;
using CalculatorService.Server.Models;

namespace CalculatorService.Server.Services
{
    public class JournalService
    {
        // Usamos una estructura que soporte hilos (thread-safe)
        private readonly ConcurrentDictionary<string, List<JournalEntry>> _journal = new();

        public void AddEntry(string id, string op, string calc)
        {
            var entry = new JournalEntry
            {
                Operation = op,
                Calculation = calc,
                Date = DateTime.Now // Usamos DateTime.Now para que coincida con tu hora local
            };

            _journal.AddOrUpdate(id,
                // Si el ID no existe, creamos una lista nueva
                _ => new List<JournalEntry> { entry },
                // Si existe, bloqueamos la lista un segundo para añadir el dato sin errores
                (_, list) =>
                {
                    lock (list)
                    {
                        list.Add(entry);
                    }
                    return list;
                });
        }

        public List<JournalEntry> GetEntries(string id)
        {
            // Devolvemos una copia de la lista para evitar errores mientras el usuario la lee
            if (_journal.TryGetValue(id, out var list))
            {
                lock (list)
                {
                    return new List<JournalEntry>(list);
                }
            }
            return new List<JournalEntry>();
        }
    }
}