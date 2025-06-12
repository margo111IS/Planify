using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planify.Models;

namespace Planify.Data
{
    public class EventRepository
    {
        private const string FilePath = "events.json";
        public static List<Event> LoadEvents()
        {
            if (!File.Exists(FilePath) || new FileInfo(FilePath).Length == 0)
            {
                return new List<Event>();
            }
            var json = File.ReadAllText(FilePath);
            return System.Text.Json.JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
        }

        public static void AddEvents(List<Event> events)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(events);
            File.WriteAllText(FilePath, json);
        }
    }
}