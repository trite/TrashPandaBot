using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashPandaBot.Data.Calendar
{
    public class CalendarEvent
    {
        //public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        public string Person { get; set; }

        public CalendarEventType EventType { get; set; }
    }
}
