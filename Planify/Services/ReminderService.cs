using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planify.Data;
using Planify.Models;

namespace Planify.Services
{
    static public class ReminderService
    {
        public static void ViewReminders(List<Event> events, List<Event> pendingReminders, HashSet<int>? mutedRemindersIds)
        {
            Console.WriteLine("\n=== Pending Reminders ===\n");
            foreach (var reminder in pendingReminders)
            {
                StringBuilder output = new StringBuilder();
                output.Append($"ID: {reminder.Id}, Title: {reminder.Title}, Start Date: {reminder.StartDate.ToString("dd/MM/yyyy HH:mm")}, Duration: {reminder.DurationInHours} hours");
                if (!string.IsNullOrWhiteSpace(reminder.Description)) output.Append($", Description: {reminder.Description}");
                if (!string.IsNullOrWhiteSpace(reminder.Location)) output.Append($", Location: {reminder.Location}");
                Console.WriteLine(output.ToString());
                Console.WriteLine();
            }

            Console.Write("Enter ID of a reminder to manage it: ");
            string? input = Console.ReadLine()?.Trim();


            if (int.TryParse(input, out int idToManage) && idToManage >= 1 && idToManage <= events.Count)
            {
                ManageReminder(events, events[idToManage - 1], mutedRemindersIds);
            }
            else
            {
                Console.WriteLine("Invalid ID. Returning to main menu.\n");
            }
        }

        public static void ManageReminder(List<Event> events, Event reminder, HashSet<int>? mutedRemindersIds)
        {
            Console.WriteLine("\nManage Reminder\n");
            Console.WriteLine($"Event: {reminder.Title}, Start Date: {reminder.StartDate.ToString("dd/MM/yyyy HH:mm")}\n");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Mute Reminder (hidden until restart)\n2. Discard Reminder (will not show again)\n3. Back");
            Console.Write("Choice: ");
            string? choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    mutedRemindersIds.Add(reminder.Id);
                    Console.WriteLine("Reminder muted.");
                    break;
                case "2":
                    reminder.IsReminderDiscarded = true;
                    Console.WriteLine("Reminder discarded.");
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            EventRepository.AddEvents(events);
            Console.WriteLine("Reminder status updated successfully!\n");
        }

        public static void AddReminderToEvent(List<Event> events)
        {
            Console.WriteLine("Toggle Reminder\n");

            foreach (var ev in events.OrderBy(e => e.Id))
            {
                Console.WriteLine($"ID: {ev.Id}, Title: {ev.Title}, Reminder: {(ev.NeedsReminder ? "ON" : "OFF")}");
            }

            Console.Write("\nEnter ID of the event to toggle reminder: ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out int id))
            {
                var selected = events.FirstOrDefault(e => e.Id == id);
                if (selected != null)
                {
                    selected.NeedsReminder = !selected.NeedsReminder;
                    if (selected.NeedsReminder)
                        selected.IsReminderDiscarded = false; // reset discard if turning on

                    EventRepository.AddEvents(events);
                    Console.WriteLine($"Reminder turned {(selected.NeedsReminder ? "ON" : "OFF")} for '{selected.Title}'.\n");
                }
                else
                {
                    Console.WriteLine("Event not found.\n");
                }
            }
            else
            {
                Console.WriteLine("Invalid input.\n");
            }
        }
    }
}