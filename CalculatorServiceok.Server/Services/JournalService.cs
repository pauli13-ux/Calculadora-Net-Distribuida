using CalculatorServiceok.Server.Models;
using System.Collections.Concurrent;

namespace CalculatorServiceok.Server.Services
{
    public class JournalService
    {
        private static readonly ConcurrentDictionary<string, List<JournalEntry>> _journals = new();

        public void AddEntry(string id, string operation, string calculation)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            // Si el ID no existe, lo crea. Si existe, lo obtiene.
            var list = _journals.GetOrAdd(id, _ => new List<JournalEntry>());
            lock (list)
            {
                list.Add(new JournalEntry { Operation = operation, Calculation = calculation, Date = DateTime.Now });
            }
        }

        public List<JournalEntry> GetEntries(string id)
        {
            // NUNCA fallará con "Key not present" porque usamos TryGetValue
            if (!string.IsNullOrWhiteSpace(id) && _journals.TryGetValue(id, out var entries))
            {
                return entries.ToList();
            }
            return new List<JournalEntry>(); // Devuelve lista vacía, nunca error
        }
    }
}