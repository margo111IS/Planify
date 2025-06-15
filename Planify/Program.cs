using System.ComponentModel.Design;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Text;
using Planify.Data;
using Planify.Models;
using Planify.Services;

public class Program
{
    static HashSet<int> mutedReminderIds;
    public static void Main(string[] args)
    {
        List<Event> events = EventRepository.LoadEvents();
        Console.WriteLine("\nWelcome to Planify!\n");
        mutedReminderIds = new HashSet<int>();
        while (true)
        {
            var pendingReminders = events.Where(e => e.NeedsReminder && !mutedReminderIds.Contains(e.Id) && !e.IsReminderDiscarded && e.StartDate > DateTime.Now && (e.StartDate - DateTime.Now).TotalHours <= 24).ToList();

            Console.WriteLine("Choose an option: \n1. Add Event\n2. View Events\n3. Clear file\n4. Exit \n5. Add reminder to an event \n6. Delete event");
            if (pendingReminders.Count > 0)
            {
                Console.Write("7. View Pending Reminders\n");
                Console.WriteLine($"\nYou have {pendingReminders.Count} upcoming event(s) that will happen within the next 24 hours.");
            }
            Console.Write("\nChoice: ");
            var input = Console.ReadLine()?.Trim();
            switch (input)
            {
                case "Add":
                case "1":
                case "Add Event":
                    EventService.AddEvents(events);
                    break;
                case "View":
                case "2":
                case "View Events":
                    EventService.ViewEvents(events);
                    break;
                case "Clear":
                case "3":
                case "Clear file":
                    Console.WriteLine("Are you sure you want to clear all events? (y/n)");
                    string? confirm = Console.ReadLine();
                    if (confirm?.Trim().ToLower() == "y")
                    {
                        File.WriteAllText("events.json", string.Empty);
                        events.Clear();
                        Console.WriteLine("All events cleared successfully!\n");
                    }
                    else
                    {
                        Console.WriteLine("Clear operation cancelled.");
                    }
                    break;
                case "Exit":
                case "4":
                    return;

                case "Add reminder to an event:":
                case "5":
                    ReminderService.AddReminderToEvent(events);
                    break;

                case "Delete event":
                case "6":
                    Console.WriteLine("Enter the ID of the event you want to delete: ");
                    string? deleteInput = Console.ReadLine()?.Trim();
                    if (int.TryParse(deleteInput, out int eventId) && eventId >= 1 && eventId <= events.Count)
                    {
                        EventService.DeleteEvent(events, eventId);
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID. Please try again.");
                    }
                    break;
                case "View Pending Reminders":
                case "7":
                    if (pendingReminders.Any())
                    {
                        ReminderService.ViewReminders(events, pendingReminders, mutedReminderIds);
                    }
                    else
                    {
                        Console.WriteLine("No pending reminders found.\n");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
        }
    }
}


