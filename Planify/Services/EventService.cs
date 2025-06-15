using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planify.Data;
using Planify.Models;

namespace Planify.Services
{
    static public class EventService
    {
        public static void AddEvents(List<Event> events)
        {
            //Title
            string? title;
            while (true)
            {
                Console.WriteLine("\nEnter event title: ");
                title = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(title))
                    break;
                Console.WriteLine("Title cannot be empty. Please enter a valid title: ");
            }

            //Description
            Console.WriteLine("Enter event description (optional): ");
            string? description = Console.ReadLine();

            // Start Date
            DateTime startDate;
            while (true)
            {
                Console.WriteLine("Enter event start date (yyyy-mm-dd HH:mm): ");
                var input = Console.ReadLine()?.Trim();
                if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    break;
                Console.WriteLine("Invalid date format.. Use yyyy-MM-dd HH:mm (e.g., 2025-06-15 14:30).");
            }

            // Duration
            Console.WriteLine("Enter event duration (in hours): ");
            string? durationInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(durationInput))
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            durationInput = durationInput.Replace(',', '.');
            if (!double.TryParse(durationInput, NumberStyles.Number, CultureInfo.InvariantCulture, out double durationHours) || durationHours <= 0)
            {
                Console.WriteLine("Invalid duration.");
                return;
            }

            // Location
            Console.WriteLine("Enter event location (optional): ");
            string? location = Console.ReadLine();

            // Reminder
            Console.WriteLine("Does the event need a reminder? (yes/no): ");
            string? reminderInput = Console.ReadLine();
            bool needsReminder = reminderInput?.Trim().ToLower() == "yes";

            //Id
            int newId = events.Count > 0 ? events.Max(e => e.Id) + 1 : 1;

            Event newEvent = new Event
            {
                Id = newId,
                Title = title,
                Description = description,
                StartDate = startDate,
                DurationInHours = durationHours,
                Location = location,
                NeedsReminder = needsReminder
            };
            events.Add(newEvent);
            EventRepository.AddEvents(events);
            Console.WriteLine("Event added successfully!\n");
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
                StringBuilder output = new StringBuilder();
                output.Append($"ID: {ev.Id}, Title: {ev.Title}, Start Date: {ev.StartDate.ToString("dd/MM/yyyy HH:mm")}, Duration: {ev.DurationInHours} hours, Needs Reminder: {ev.NeedsReminder}");
                if (!string.IsNullOrWhiteSpace(ev.Description)) output.Append($", Description: {ev.Description}");
                if (!string.IsNullOrWhiteSpace(ev.Location)) output.Append($", Location: {ev.Location}");
                Console.WriteLine(output.ToString());
                Console.WriteLine();
            }
        }

        public static void DeleteEvent(List<Event> events, int eventId)
        {
            var eventToDelete = events.FirstOrDefault(e => e.Id == eventId);
            if (eventToDelete != null)
            {
                events.Remove(eventToDelete);
                EventRepository.AddEvents(events);
                Console.WriteLine($"Event with ID {eventId} deleted successfully.\n");
            }
            else
            {
                Console.WriteLine($"Event with ID {eventId} not found.\n");
            }
        }
    }
}