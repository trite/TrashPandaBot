using Discord.Commands;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrashPandaBot.Data.Calendar;
using TriteUtilities.Azure.Blob;

namespace TrashPandaBot
{
    public class CalendarModule : ModuleBase<SocketCommandContext>
    {
        [Command("late")]
        [Summary("Mark yourself as late for a raid")]
        public Task LateAsync()
        {
            return LateAsync("");
        }

        [Command("late")]
        [Summary("Mark yourself as late for a raid")]
        public Task LateAsync([Remainder] string info)
        {
            //if (string.IsNullOrWhiteSpace(info))
            //{
            // do just today
            var userInfo = Context.User;

            DateTime now = DateTime.Now;
            string containerName = "calendar";
            string blobName = $"events_{now.Year}-{now.Month}";

            var eventsBlob = BlobStorageService.ReadBlob("TrashPandaConn", containerName, blobName);
            var eventsDict = eventsBlob.Map(b => JsonSerializer.Deserialize<Dictionary<int, List<CalendarEvent>>>(b))
                .IfNone(() => new Dictionary<int, List<CalendarEvent>>());

            if (!eventsDict.ContainsKey(now.Day))
            {
                eventsDict.Add(now.Day, new());
            }

            List<CalendarEvent> eventsForDay = eventsDict[now.Day];

            CalendarEvent evt = eventsForDay.Where(c => c.Person == userInfo.Username).FirstOrDefault();

            if (evt is null)
            {
                CalendarEvent newEvent = new()
                {
                    Person = userInfo.Username,
                    Description = $"{userInfo.Username} - late",
                    Details = $"{userInfo.Username} - late",
                    EventType = CalendarEventType.AttendanceLate
                };
                eventsDict[now.Day].Add(newEvent);
            }
            else
            {
                evt.Description = $"{userInfo.Username} - late";
                evt.Details = $"{userInfo.Username} - late";
                evt.EventType = CalendarEventType.AttendanceLate;
            }

            var newBlobValue = JsonSerializer.Serialize(eventsDict);
            var result = BlobStorageService.WriteBlob("TrashPandaConn", containerName, blobName, newBlobValue);

            var response = result
                ? "You are now marked as being late for raid today. https://trashpandasite.azurewebsites.net/"
                : "Could not mark you late on the calendar! Tell Trite to FIX IT.";

            return ReplyAsync(response);
        }

        [Command("absent")]
        [Summary("Mark yourself as absent for a raid")]
        public Task AbsentAsync()
        {
            return AbsentAsync("");
        }

        [Command("absent")]
        [Summary("Mark yourself as absent for a raid")]
        public Task AbsentAsync([Remainder] string info)
        {
            //if (string.IsNullOrWhiteSpace(info))
            //{
            // do just today
            var userInfo = Context.User;

            DateTime now = DateTime.Now;
            string containerName = "calendar";
            string blobName = $"events_{now.Year}-{now.Month}";

            var eventsBlob = BlobStorageService.ReadBlob("TrashPandaConn", containerName, blobName);
            var eventsDict = eventsBlob.Map(b => JsonSerializer.Deserialize<Dictionary<int, List<CalendarEvent>>>(b))
                .IfNone(() => new Dictionary<int, List<CalendarEvent>>());

            if (!eventsDict.ContainsKey(now.Day))
            {
                eventsDict.Add(now.Day, new());
            }

            List<CalendarEvent> eventsForDay = eventsDict[now.Day];

            CalendarEvent evt = eventsForDay.Where(c => c.Person == userInfo.Username).FirstOrDefault();

            if (evt is null)
            {
                CalendarEvent newEvent = new()
                {
                    Person = userInfo.Username,
                    Description = $"{userInfo.Username} - absent",
                    Details = $"{userInfo.Username} - absent",
                    EventType = CalendarEventType.AttendanceAbsent
                };
                eventsDict[now.Day].Add(newEvent);
            }
            else
            {
                evt.Description = $"{userInfo.Username} - absent";
                evt.Details = $"{userInfo.Username} - absent";
                evt.EventType = CalendarEventType.AttendanceAbsent;
            }            

            var newBlobValue = JsonSerializer.Serialize(eventsDict);
            var result = BlobStorageService.WriteBlob("TrashPandaConn", containerName, blobName, newBlobValue);

            var response = result
                ? "You are now marked as being absent for raid today. https://trashpandasite.azurewebsites.net/"
                : "Could not mark you absent on the calendar! Tell Trite to FIX IT.";

            return ReplyAsync(response);
        }

        [Command("clear")]
        [Summary("Clears you of any raid attendance status")]
        public Task ClearAsync()
        {
            return ClearAsync("");
        }

        [Command("clear")]
        [Summary("Clears you of any raid attendance status")]
        public Task ClearAsync([Remainder] string info)
        {
            var userInfo = Context.User;

            DateTime now = DateTime.Now;
            string containerName = "calendar";
            string blobName = $"events_{now.Year}-{now.Month}";

            var eventsBlob = BlobStorageService.ReadBlob("TrashPandaConn", containerName, blobName);
            var eventsDict = eventsBlob.Map(b => JsonSerializer.Deserialize<Dictionary<int, List<CalendarEvent>>>(b))
                .IfNone(() => new Dictionary<int, List<CalendarEvent>>());

            if (!eventsDict.ContainsKey(now.Day))
            {
                eventsDict.Add(now.Day, new());
            }

            List<CalendarEvent> eventsForDay = eventsDict[now.Day];

            CalendarEvent evt = eventsForDay.Where(c => c.Person == userInfo.Username).FirstOrDefault();

            if (evt is not null)
            {
                eventsForDay.Remove(evt);
            }

            var newBlobValue = JsonSerializer.Serialize(eventsDict);
            var result = BlobStorageService.WriteBlob("TrashPandaConn", containerName, blobName, newBlobValue);

            var response = result
                ? "You have successfully cleared your attendance status. https://trashpandasite.azurewebsites.net/"
                : "Could not clear your attendance status on the calendar! Tell Trite to FIX IT.";

            return ReplyAsync(response);
        }
    }
}
