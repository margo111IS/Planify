using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Planify.Data;
using Planify.Models;

namespace Planify.Services
{
    public static class EventService
    {
        public static void AddEvents(List<Event> events)
        {
            // Title
            if (!TryReadInput(2, out string? title, "\nEnter event title: ") || string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                return;
            }

            // Description (optional)
            Console.Write("\nEnter event description (optional): ");
            string? description = "";
            try { description = Console.ReadLine(); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading input: {ex.Message}");
            }

            // Start Date
            if (!TryReadInput(2, out string? dateInput, "\nEnter start date (yyyy-MM-dd HH:mm): ") || string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                return;
            }
            if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                Console.WriteLine("Incorrect data format. Returning to menu.\n");
                return;
            }

            // Duration
            if (!TryReadInput(2, out string? duationInput, "\nEnter duration (in hours): ") || string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                return;
            }
            duationInput = duationInput.Replace(',', '.');
            if (!double.TryParse(duationInput, NumberStyles.Number, CultureInfo.InvariantCulture, out double durationHours) || durationHours <= 0)
            {
                Console.WriteLine("Incorrect duration input. Returning to menu.\n");
                return;
            }

            // Location (optional)
            Console.Write("\nEnter event location (optional): ");
            string? location = "";
            try { location = Console.ReadLine(); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading input: {ex.Message}");
            }
            // Reminder
            if (!TryReadInput(2, out string? reminderInput, "\nNeed a reminder? (yes/no): ") || string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                return;
            }
            reminderInput = reminderInput.ToLower();
            if (reminderInput != "yes" && reminderInput != "no")
            {
                Console.WriteLine("Input should be \"yes\" or \"no\" only. Returning to menu.\n");
                return;
            }
            bool needsReminder = reminderInput == "yes";

            int newId = events.Count > 0 ? events.Max(e => e.Id) + 1 : 1;
            events.Add(new Event
            {
                Id = newId,
                Title = title,
                Description = description,
                StartDate = startDate,
                DurationInHours = durationHours,
                Location = location,
                NeedsReminder = needsReminder
            });

            EventRepository.AddEvents(events);
            Console.WriteLine("\nEvent added successfully!\n");
        }

        public static void ViewEvents(List<Event> events)
        {
            Console.WriteLine("\n=== Upcoming Events ===\n");
            if (events.Count == 0)
            {
                Console.WriteLine("No events found.\n");
                return;
            }

            foreach (var ev in events.OrderBy(e => e.StartDate))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"ID: {ev.Id}, Title: {ev.Title}, Start: {ev.StartDate:dd/MM/yyyy HH:mm}, Duration: {ev.DurationInHours}h, Reminder: {ev.NeedsReminder}");
                if (!string.IsNullOrWhiteSpace(ev.Description)) sb.Append($", Desc: {ev.Description}");
                if (!string.IsNullOrWhiteSpace(ev.Location)) sb.Append($", Loc: {ev.Location}");

                Console.WriteLine(sb.ToString());
                Console.WriteLine();
            }
        }

        public static void DeleteEvent(List<Event> events, int id)
        {
            var ev = events.FirstOrDefault(e => e.Id == id);
            if (ev != null)
            {
                events.Remove(ev);
                EventRepository.AddEvents(events);
                Console.WriteLine($"Event {id} deleted.\n");
            }
            else
            {
                Console.WriteLine($"Event {id} not found.\n");
            }
        }

        public static void DeleteAllPastEvents(List<Event> events)
        {
            int removed = events.RemoveAll(e => e.StartDate < DateTime.Now);
            EventRepository.AddEvents(events);
            Console.WriteLine(removed > 0 ? $"Deleted {removed} past event(s)." : "No past events found.");
        }

        private static bool TryReadInput(int maxAttempts, out string? result, string prompt = null, bool requireNonEmpty = false)
        {
            result = null;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                if (!string.IsNullOrWhiteSpace(prompt)) Console.Write(prompt);
                try
                {
                    result = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(result))
                        return true;
                    Console.WriteLine("Input cannot be empty.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Input error: {ex.Message}");
                }
            }
            return false;
        }
    }
}
