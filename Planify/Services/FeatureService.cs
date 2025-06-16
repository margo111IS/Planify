using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planify.Models;

namespace Planify.Services
{
    public static class FeatureService
    {
        public static void FeaturesHandler(List<Event> events)
        {
            while (true)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Delete an event");
                Console.WriteLine("2. Turn on/off a reminder");
                Console.WriteLine($"3. Delete all past events (before {DateTime.Now:dd/MM/yyyy})");
                Console.WriteLine("4. Back");

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
                        case "Delete event":
                        case "1":
                            Console.Write("Enter the ID of the event you want to delete: ");
                            string? deleteInput = null;
                            try
                            {
                                deleteInput = Console.ReadLine()?.Trim();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Input error: {ex.Message}");
                                break;
                            }

                            if (int.TryParse(deleteInput, out int eventId) && events.Any(e => e.Id == eventId))
                            {
                                EventService.DeleteEvent(events, eventId);
                            }
                            else
                            {
                                Console.WriteLine("Invalid ID. Please try again.");
                            }
                            break;

                        case "Turn on/off a reminder":
                        case "2":
                            ReminderService.AddReminderToEvent(events);
                            break;

                        case "Delete all past events":
                        case "3":
                            EventService.DeleteAllPastEvents(events);
                            break;

                        case "Back":
                        case "4":
                            return;

                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"An error occured: {ex.Message}");
                }
            }
        }
    }
}