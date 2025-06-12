using System.Globalization;
using System.Text;
using Planify.Data;
using Planify.Models;

public class Program
{
    public static void Main(string[] args)
    {
        List<Event> events = EventRepository.LoadEvents();
        Console.WriteLine("\nWelcome to Planify!\n");

        while (true)
        {
            var pendingReminders = events.Where(e => e.NeedsReminder && !e.IsReminderMuted && !e.IsReminderDiscarded && e.StartDate > DateTime.Now && (e.StartDate - DateTime.Now).TotalHours <= 24).ToList();

            Console.WriteLine("Choose an option: \n1. Add Event\n2. View Events\n3. Clear file\n4. Exit");
            if (pendingReminders.Count > 0)
            {
                Console.Write("5. View Pending Reminders\n");
                Console.WriteLine($"\nYou have {pendingReminders.Count} upcoming event(s) that will happen within the next 24 hours.");
            }
            Console.Write("\nChoice: ");
            var input = Console.ReadLine()?.Trim();
            switch (input)
            {
                case "Add":
                case "1":
                case "Add Event":
                    AddEvents(events);
                    break;
                case "View":
                case "2":
                case "View Events":
                    ViewEvents(events);
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

                // case "View Pending Reminders":
                // case "5":
                //     if (pendingReminders.Any())
                //     {
                //         ViewReminders(pendingReminders);
                //     }
                //     else
                //     {
                //         Console.WriteLine("No pending reminders found.\n");
                //     }
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
        }
    }

    public static void AddEvents(List<Event> events)
    {
        //Title
        string title;
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

    public static void ViewReminders(List<Event> pendingReminders)
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
    }
}
