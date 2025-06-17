using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planify.Models;

namespace Planify.Data
{
    public class EventRepository
    {
        private static readonly string planifyPath;
        private static readonly string filePath;

        static EventRepository()
        {
            var documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            planifyPath = Path.Combine(documentsFolderPath, "Planify");
            Directory.CreateDirectory(planifyPath);
            filePath = Path.Combine(planifyPath, "events.json");
        }

        public static List<Event> LoadEvents()
        {
            try
            {
                if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
                {
                    return new List<Event>();
                }
                var json = File.ReadAllText(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading events: {ex.Message}");
                return new List<Event>();
            }
        }

        public static void AddEvents(List<Event> events)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(events);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving events: {ex.Message}");
            }
        }
    }
}