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

