using System.Globalization;
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
            var pendingReminders = events
                .Where(e =>
                    e.NeedsReminder &&
                    !mutedReminderIds.Contains(e.Id) &&
                    !e.IsReminderDiscarded &&
                    e.StartDate > DateTime.Now &&
                    (e.StartDate - DateTime.Now).TotalHours <= 24
                )
                .ToList();

            // Menu display
            Console.WriteLine("\nChoose an option:");
            Console.WriteLine("1. Add Event");
            Console.WriteLine("2. View Events");
            Console.WriteLine("3. Clear file");
            Console.WriteLine("4. Other");
            Console.WriteLine("5. Exit");

            if (pendingReminders.Count > 0)
            {
                Console.WriteLine("6. View Pending Reminders");
                Console.WriteLine($"\nYou have {pendingReminders.Count} upcoming event(s) that will happen within the next 24 hours.");
            }

            Console.Write("\nChoice: ");
            string? input = null;
            try
            {
                input = Console.ReadLine()?.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Input error: {ex.Message}");
                continue;
            }

            try
            {
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
                        string? confirm = null;
                        try
                        {
                            confirm = Console.ReadLine();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Input error: {ex.Message}");
                            break;
                        }

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

                    case "Other":
                    case "4":
                        FeatureService.FeaturesHandler(events);
                        break;

                    case "Exit":
                    case "5":
                        return;

                    case "View Pending Reminders":
                    case "6":
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
                        Console.WriteLine("Invalid option, please try again.\n");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
