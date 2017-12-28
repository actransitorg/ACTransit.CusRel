using System;
using System.Linq;

namespace ACTransit.CusRel.Services.Extensions
{
    public class DateTimeExtensions
    {
        private readonly MapsScheduleService mapsScheduleService;
        public DateTimeExtensions(MapsScheduleService mapsScheduleService)
        {
            this.mapsScheduleService = mapsScheduleService;
        }

        public DateTime PreviousWorkDay(DateTime date)
        {
            do
            {
                date = date.AddDays(-1);
            } while (IsHoliday(date) || IsWeekend(date));

            return date;
        }

        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public bool IsHoliday(DateTime date)
        {
            var holidays = mapsScheduleService.GetHolidayList();
            return holidays.Any(h => h.Date.Equals(date.Date));
        }
    }
}
