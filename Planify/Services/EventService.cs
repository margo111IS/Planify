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
            string? title;
            int titleTries = 0;
            while (true)
            {
                Console.Write("\nEnter event title: ");
                try
                {
                    title = Console.ReadLine()?.Trim();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading input: {ex.Message}");
                    titleTries++;
                    if (titleTries >= 2)
                    {
                        Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                        return;
                    }
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(title))
                    break;

                Console.WriteLine("Title cannot be empty.");
                titleTries++;
                if (titleTries >= 2)
                {
                    Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                    return;
                }
            }

            // Description
            Console.Write("\nEnter event description (optional): ");
            string? description = "";
            try { description = Console.ReadLine(); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading input: {ex.Message}");
            }

            // Start Date
            DateTime startDate;
            int dateTries = 0;
            while (true)
            {
                Console.Write("\nEnter event start date (yyyy-MM-dd HH:mm): ");
                string? input;
                try { input = Console.ReadLine()?.Trim(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading input: {ex.Message}");
                    dateTries++;
                    if (dateTries >= 2)
                    {
                        Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                        return;
                    }
                    continue;
                }

                if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    break;

                Console.WriteLine("Invalid date format. Use yyyy-MM-dd HH:mm (e.g., 2025-06-15 14:30).");
                dateTries++;
                if (dateTries >= 2)
                {
                    Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                    return;
                }
            }

            // Duration
            double durationHours;
            int durationTries = 0;
            while (true)
            {
                Console.Write("\nEnter event duration (in hours): ");
                string? durationInput;
                try { durationInput = Console.ReadLine()?.Trim(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading input: {ex.Message}");
                    durationTries++;
                    if (durationTries >= 2)
                    {
                        Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                        return;
                    }
                    continue;
                }

                durationInput = durationInput?.Replace(',', '.');
                if (double.TryParse(durationInput, NumberStyles.Number, CultureInfo.InvariantCulture, out durationHours) && durationHours > 0)
                    break;

                Console.WriteLine("Invalid duration. Please enter a number greater than 0.");
                durationTries++;
                if (durationTries >= 2)
                {
                    Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                    return;
                }
            }

            // Location
            Console.Write("\nEnter event location (optional): ");
            string? location = "";
            try { location = Console.ReadLine(); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading input: {ex.Message}");
            }

            // Reminder
            bool needsReminder;
            int reminderTries = 0;
            while (true)
            {
                Console.Write("\nDoes the event need a reminder? (yes/no): ");
                string? reminderInput;
                try { reminderInput = Console.ReadLine()?.Trim().ToLower(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading input: {ex.Message}");
                    reminderTries++;
                    if (reminderTries >= 2)
                    {
                        Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                        return;
                    }
                    continue;
                }

                if (reminderInput == "yes")
                {
                    needsReminder = true;
                    break;
                }
                else if (reminderInput == "no")
                {
                    needsReminder = false;
                    break;
                }

                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                reminderTries++;
                if (reminderTries >= 2)
                {
                    Console.WriteLine("Too many failed attempts. Returning to menu.\n");
                    return;
                }
            }

            // ID generation
            int newId = events.Count > 0 ? events.Max(e => e.Id) + 1 : 1;

            // Create and save
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
                StringBuilder output = new StringBuilder();
                output.Append($"ID: {ev.Id}, Title: {ev.Title}, Start Date: {ev.StartDate:dd/MM/yyyy HH:mm}, Duration: {ev.DurationInHours} hours, Needs Reminder: {ev.NeedsReminder}");

                if (!string.IsNullOrWhiteSpace(ev.Description))
                    output.Append($", Description: {ev.Description}");
                if (!string.IsNullOrWhiteSpace(ev.Location))
                    output.Append($", Location: {ev.Location}");

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

        public static void DeleteAllPastEvents(List<Event> events)
        {
            int removedEventsCount = events.RemoveAll(e => e.StartDate < DateTime.Now);
            EventRepository.AddEvents(events);
            Console.WriteLine(removedEventsCount > 0 ? $"Deleted {removedEventsCount} event(s)." : "No past events found.");
        }
    }
}
